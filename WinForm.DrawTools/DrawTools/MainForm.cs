using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using System.Security;
using System.Windows.Forms;

using DocToolkit;

using Microsoft.Win32;

namespace DrawTools
{
	internal partial class MainForm : Form
	{
		#region Members
		private DrawArea drawArea;
		private DocManager docManager;
		private DragDropManager dragDropManager;
		private MruManager mruManager;

		private string argumentFile = ""; // file name from command line

		private const string registryPath = "Software\\AlexF\\DrawTools";

		private bool _controlKey = false;
		private bool _panMode = false;
		#endregion

		#region Properties
		/// <summary>
		/// File name from the command line
		/// </summary>
		public string ArgumentFile
		{
			get { return argumentFile; }
			set { argumentFile = value; }
		}

		/// <summary>
		/// Get reference to Edit menu item.
		/// Used to show context menu in DrawArea class.
		/// </summary>
		/// <value></value>
		public ToolStripMenuItem ContextParent
		{
			get { return editToolStripMenuItem; }
		}
		#endregion

		#region Constructor
		public MainForm()
		{
			InitializeComponent();
			MouseWheel += new MouseEventHandler(MainForm_MouseWheel);
		}
		#endregion

		#region Toolbar Event Handlers
		private void toolStripButtonNew_Click(object sender, EventArgs e)
		{
			CommandNew();
		}

		private void toolStripButtonOpen_Click(object sender, EventArgs e)
		{
			CommandOpen();
		}

		private void toolStripButtonSave_Click(object sender, EventArgs e)
		{
			CommandSave();
		}

		private void toolStripButtonPointer_Click(object sender, EventArgs e)
		{
			CommandPointer();
		}

		private void toolStripButtonRectangle_Click(object sender, EventArgs e)
		{
			CommandRectangle();
		}

		private void toolStripButtonEllipse_Click(object sender, EventArgs e)
		{
			CommandEllipse();
		}

		private void toolStripButtonLine_Click(object sender, EventArgs e)
		{
			CommandLine();
		}

		private void toolStripButtonPencil_Click(object sender, EventArgs e)
		{
			CommandPolygon();
		}

		private void toolStripButtonAbout_Click(object sender, EventArgs e)
		{
			CommandAbout();
		}

		private void toolStripButtonUndo_Click(object sender, EventArgs e)
		{
			CommandUndo();
		}

		private void toolStripButtonRedo_Click(object sender, EventArgs e)
		{
			CommandRedo();
		}
		#endregion Toolbar Event Handlers

		#region Menu Event Handlers
		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandNew();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandOpen();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandSave();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandSaveAs();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int x = drawArea.TheLayers.ActiveLayerIndex;
			drawArea.TheLayers[x].Graphics.SelectAll();
			drawArea.Refresh();
		}

		private void unselectAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int x = drawArea.TheLayers.ActiveLayerIndex;
			drawArea.TheLayers[x].Graphics.UnselectAll();
			drawArea.Refresh();
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int x = drawArea.TheLayers.ActiveLayerIndex;
			CommandDelete command = new CommandDelete(drawArea.TheLayers);

			if (drawArea.TheLayers[x].Graphics.DeleteSelection())
			{
				drawArea.Refresh();
				drawArea.AddCommandToHistory(command);
			}
		}

		private void deleteAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int x = drawArea.TheLayers.ActiveLayerIndex;
			CommandDeleteAll command = new CommandDeleteAll(drawArea.TheLayers);

			if (drawArea.TheLayers[x].Graphics.Clear())
			{
				drawArea.Refresh();
				drawArea.AddCommandToHistory(command);
			}
		}

		private void moveToFrontToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int x = drawArea.TheLayers.ActiveLayerIndex;
			if (drawArea.TheLayers[x].Graphics.MoveSelectionToFront())
			{
				drawArea.Refresh();
			}
		}

		private void moveToBackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			int x = drawArea.TheLayers.ActiveLayerIndex;
			if (drawArea.TheLayers[x].Graphics.MoveSelectionToBack())
			{
				drawArea.Refresh();
			}
		}

		private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//if (drawArea.GraphicsList.ShowPropertiesDialog(drawArea))
			//{
			//    drawArea.SetDirty();
			//    drawArea.Refresh();
			//}
		}

		private void pointerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandPointer();
		}

		private void rectangleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandRectangle();
		}

		private void ellipseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandEllipse();
		}

		private void lineToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandLine();
		}

		private void pencilToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandPolygon();
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandAbout();
		}

		private void undoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandUndo();
		}

		private void redoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandRedo();
		}
		#endregion Menu Event Handlers

		#region DocManager Event Handlers
		/// <summary>
		/// Load document from the stream supplied by DocManager
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void docManager_LoadEvent(object sender, SerializationEventArgs e)
		{
			// DocManager asks to load document from supplied stream
			try
			{
				drawArea.TheLayers = (Layers)e.Formatter.Deserialize(e.SerializationStream);
			} catch (ArgumentNullException ex)
			{
				HandleLoadException(ex, e);
			} catch (SerializationException ex)
			{
				HandleLoadException(ex, e);
			} catch (SecurityException ex)
			{
				HandleLoadException(ex, e);
			}
		}


		/// <summary>
		/// Save document to stream supplied by DocManager
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void docManager_SaveEvent(object sender, SerializationEventArgs e)
		{
			// DocManager asks to save document to supplied stream
			try
			{
				e.Formatter.Serialize(e.SerializationStream, drawArea.TheLayers);
			} catch (ArgumentNullException ex)
			{
				HandleSaveException(ex, e);
			} catch (SerializationException ex)
			{
				HandleSaveException(ex, e);
			} catch (SecurityException ex)
			{
				HandleSaveException(ex, e);
			}
		}
		#endregion

		#region Event Handlers
		private void MainForm_Load(object sender, EventArgs e)
		{
			// Create draw area
			drawArea = new DrawArea();
			drawArea.Location = new Point(0, 0);
			drawArea.Size = new Size(10, 10);
			drawArea.Owner = this;
			Controls.Add(drawArea);

			// Helper objects (DocManager and others)
			InitializeHelperObjects();

			drawArea.Initialize(this, docManager);
			ResizeDrawArea();

			LoadSettingsFromRegistry();

			// Submit to Idle event to set controls state at idle time
			Application.Idle += delegate { SetStateOfControls(); };

			// Open file passed in the command line
			if (ArgumentFile.Length > 0)
				OpenDocument(ArgumentFile);

			// Subscribe to DropDownOpened event for each popup menu
			// (see details in MainForm_DropDownOpened)
			foreach (ToolStripItem item in menuStrip1.Items)
			{
				if (item.GetType() ==
					typeof(ToolStripMenuItem))
				{
					((ToolStripMenuItem)item).DropDownOpened += MainForm_DropDownOpened;
				}
			}
		}

		/// <summary>
		/// Resize draw area when form is resized
		/// </summary>
		private void MainForm_Resize(object sender, EventArgs e)
		{
			if (WindowState != FormWindowState.Minimized &&
				drawArea != null)
			{
				ResizeDrawArea();
			}
		}

		/// <summary>
		/// Form is closing
		/// </summary>
		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason ==
				CloseReason.UserClosing)
			{
				if (!docManager.CloseDocument())
					e.Cancel = true;
			}

			SaveSettingsToRegistry();
		}

		/// <summary>
		/// Popup menu item (File, Edit ...) is opened.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainForm_DropDownOpened(object sender, EventArgs e)
		{
			// Reset active tool to pointer.
			// This prevents bug in rare case when non-pointer tool is active, user opens
			// main main menu and after this clicks in the drawArea. MouseDown event is not
			// raised in this case (why ??), and MouseMove event works incorrectly.
			drawArea.ActiveTool = DrawArea.DrawToolType.Pointer;
		}
		#endregion Event Handlers

		#region Other Functions
		/// <summary>
		/// Set state of controls.
		/// Function is called at idle time.
		/// </summary>
		public void SetStateOfControls()
		{
			// Select active tool
			toolStripButtonPointer.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Pointer);
			toolStripButtonRectangle.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Rectangle);
			toolStripButtonEllipse.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Ellipse);
			toolStripButtonLine.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Line);
			toolStripButtonPencil.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Polygon);

			pointerToolStripMenuItem.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Pointer);
			rectangleToolStripMenuItem.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Rectangle);
			ellipseToolStripMenuItem.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Ellipse);
			lineToolStripMenuItem.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Line);
			pencilToolStripMenuItem.Checked = (drawArea.ActiveTool == DrawArea.DrawToolType.Polygon);

			int x = drawArea.TheLayers.ActiveLayerIndex;
			bool objects = (drawArea.TheLayers[x].Graphics.Count > 0);
			bool selectedObjects = (drawArea.TheLayers[x].Graphics.SelectionCount > 0);
			// File operations
			saveToolStripMenuItem.Enabled = objects;
			toolStripButtonSave.Enabled = objects;
			saveAsToolStripMenuItem.Enabled = objects;

			// Edit operations
			deleteToolStripMenuItem.Enabled = selectedObjects;
			deleteAllToolStripMenuItem.Enabled = objects;
			selectAllToolStripMenuItem.Enabled = objects;
			unselectAllToolStripMenuItem.Enabled = objects;
			moveToFrontToolStripMenuItem.Enabled = selectedObjects;
			moveToBackToolStripMenuItem.Enabled = selectedObjects;
			propertiesToolStripMenuItem.Enabled = selectedObjects;

			// Undo, Redo
			undoToolStripMenuItem.Enabled = drawArea.CanUndo;
			toolStripButtonUndo.Enabled = drawArea.CanUndo;

			redoToolStripMenuItem.Enabled = drawArea.CanRedo;
			toolStripButtonRedo.Enabled = drawArea.CanRedo;

			// Status Strip
			tslCurrentLayer.Text = drawArea.TheLayers[x].LayerName;
			tslNumberOfObjects.Text = drawArea.TheLayers[x].Graphics.Count.ToString();
			tslPanPosition.Text = drawArea.PanX + ", " + drawArea.PanY;
			tslRotation.Text = drawArea.Rotation + " deg";
			tslZoomLevel.Text = (Math.Round(drawArea.Zoom * 100)) + " %";

			// Pan button
			tsbPanMode.Checked = drawArea.Panning;
		}

		/// <summary>
		/// Set draw area to all form client space except toolbar
		/// </summary>
		private void ResizeDrawArea()
		{
			Rectangle rect = ClientRectangle;

			drawArea.Left = rect.Left;
			drawArea.Top = rect.Top + menuStrip1.Height + toolStrip1.Height;
			drawArea.Width = rect.Width;
			drawArea.Height = rect.Height - menuStrip1.Height - toolStrip1.Height;
			;
		}

		/// <summary>
		/// Initialize helper objects from the DocToolkit Library.
		/// 
		/// Called from Form1_Load. Initialized all objects except
		/// PersistWindowState wich must be initialized in the
		/// form constructor.
		/// </summary>
		private void InitializeHelperObjects()
		{
			// DocManager
			DocManagerData data = new DocManagerData();
			data.FormOwner = this;
			data.UpdateTitle = true;
			data.FileDialogFilter = "DrawTools files (*.dtl)|*.dtl|All Files (*.*)|*.*";
			data.NewDocName = "Untitled.dtl";
			data.RegistryPath = registryPath;

			docManager = new DocManager(data);
			docManager.RegisterFileType("dtl", "dtlfile", "DrawTools File");

			// Subscribe to DocManager events.
			docManager.SaveEvent += docManager_SaveEvent;
			docManager.LoadEvent += docManager_LoadEvent;

			// Make "inline subscription" using anonymous methods.
			docManager.OpenEvent += delegate(object sender, OpenFileEventArgs e)
										{
											// Update MRU List
											if (e.Succeeded)
												mruManager.Add(e.FileName);
											else
												mruManager.Remove(e.FileName);
										};

			docManager.DocChangedEvent += delegate
											{
												drawArea.Refresh();
												drawArea.ClearHistory();
											};

			docManager.ClearEvent += delegate
										{
											bool haveObjects = false;
											for (int i = 0; i < drawArea.TheLayers.Count; i++)
											{
												if (drawArea.TheLayers[i].Graphics.Count > 1)
												{
													haveObjects = true;
													break;
												}
											}
											if (haveObjects)
											{
												drawArea.TheLayers.Clear();
												drawArea.ClearHistory();
												drawArea.Refresh();
											}
										};

			docManager.NewDocument();

			// DragDropManager
			dragDropManager = new DragDropManager(this);
			dragDropManager.FileDroppedEvent += delegate(object sender, FileDroppedEventArgs e) { OpenDocument(e.FileArray.GetValue(0).ToString()); };

			// MruManager
			mruManager = new MruManager();
			mruManager.Initialize(
				this, // owner form
				recentFilesToolStripMenuItem, // Recent Files menu item
				fileToolStripMenuItem, // parent
				registryPath); // Registry path to keep MRU list

			mruManager.MruOpenEvent += delegate(object sender, MruFileOpenEventArgs e) { OpenDocument(e.FileName); };
		}

		/// <summary>
		/// Handle exception from docManager_LoadEvent function
		/// </summary>
		/// <param name="ex"></param>
		/// <param name="e"></param>
		private void HandleLoadException(Exception ex, SerializationEventArgs e)
		{
			MessageBox.Show(this,
							"Open File operation failed. File name: " + e.FileName + "\n" +
							"Reason: " + ex.Message,
							Application.ProductName);

			e.Error = true;
		}

		/// <summary>
		/// Handle exception from docManager_SaveEvent function
		/// </summary>
		/// <param name="ex"></param>
		/// <param name="e"></param>
		private void HandleSaveException(Exception ex, SerializationEventArgs e)
		{
			MessageBox.Show(this,
							"Save File operation failed. File name: " + e.FileName + "\n" +
							"Reason: " + ex.Message,
							Application.ProductName);

			e.Error = true;
		}

		/// <summary>
		/// Open document.
		/// Used to open file passed in command line or dropped into the window
		/// </summary>
		/// <param name="file"></param>
		public void OpenDocument(string file)
		{
			docManager.OpenDocument(file);
		}

		/// <summary>
		/// Load application settings from the Registry
		/// </summary>
		private void LoadSettingsFromRegistry()
		{
			try
			{
				RegistryKey key = Registry.CurrentUser.CreateSubKey(registryPath);

				DrawObject.LastUsedColor = Color.FromArgb((int)key.GetValue(
																"Color",
																Color.Black.ToArgb()));

				DrawObject.LastUsedPenWidth = (int)key.GetValue(
													"Width",
													1);
			} catch (ArgumentNullException ex)
			{
				HandleRegistryException(ex);
			} catch (SecurityException ex)
			{
				HandleRegistryException(ex);
			} catch (ArgumentException ex)
			{
				HandleRegistryException(ex);
			} catch (ObjectDisposedException ex)
			{
				HandleRegistryException(ex);
			} catch (UnauthorizedAccessException ex)
			{
				HandleRegistryException(ex);
			}
		}

		/// <summary>
		/// Save application settings to the Registry
		/// </summary>
		private void SaveSettingsToRegistry()
		{
			try
			{
				RegistryKey key = Registry.CurrentUser.CreateSubKey(registryPath);

				key.SetValue("Color", DrawObject.LastUsedColor.ToArgb());
				key.SetValue("Width", DrawObject.LastUsedPenWidth);
			} catch (SecurityException ex)
			{
				HandleRegistryException(ex);
			} catch (ArgumentException ex)
			{
				HandleRegistryException(ex);
			} catch (ObjectDisposedException ex)
			{
				HandleRegistryException(ex);
			} catch (UnauthorizedAccessException ex)
			{
				HandleRegistryException(ex);
			}
		}

		private void HandleRegistryException(Exception ex)
		{
			Trace.WriteLine("Registry operation failed: " + ex.Message);
		}

		/// <summary>
		/// Set Pointer draw tool
		/// </summary>
		private void CommandPointer()
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.Pointer;
		}

		/// <summary>
		/// Set Rectangle draw tool
		/// </summary>
		private void CommandRectangle()
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.Rectangle;
			drawArea.DrawFilled = false;
		}

		/// <summary>
		/// Set Ellipse draw tool
		/// </summary>
		private void CommandEllipse()
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.Ellipse;
			drawArea.DrawFilled = false;
		}

		/// <summary>
		/// Set Line draw tool
		/// </summary>
		private void CommandLine()
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.Line;
		}

		/// <summary>
		/// Set Polygon draw tool
		/// </summary>
		private void CommandPolygon()
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.Polygon;
		}

		/// <summary>
		/// Show About dialog
		/// </summary>
		private void CommandAbout()
		{
			FrmAbout frm = new FrmAbout();
			frm.ShowDialog(this);
		}

		/// <summary>
		/// Open new file
		/// </summary>
		private void CommandNew()
		{
			docManager.NewDocument();
		}

		/// <summary>
		/// Open file
		/// </summary>
		private void CommandOpen()
		{
			docManager.OpenDocument("");
		}

		/// <summary>
		/// Save file
		/// </summary>
		private void CommandSave()
		{
			docManager.SaveDocument(DocManager.SaveType.Save);
		}

		/// <summary>
		/// Save As
		/// </summary>
		private void CommandSaveAs()
		{
			docManager.SaveDocument(DocManager.SaveType.SaveAs);
		}

		/// <summary>
		/// Undo
		/// </summary>
		private void CommandUndo()
		{
			drawArea.Undo();
		}

		/// <summary>
		/// Redo
		/// </summary>
		private void CommandRedo()
		{
			drawArea.Redo();
		}
		#endregion

		#region Mouse Functions
		private void MainForm_MouseWheel(object sender, MouseEventArgs e)
		{
			if (e.Delta != 0)
			{
				if (_controlKey)
				{
					// We are panning up or down using the wheel
					if (e.Delta > 0)
						drawArea.PanY += 10;
					else
						drawArea.PanY -= 10;
					Invalidate();
				} else
				{
					// We are zooming in or out using the wheel
					if (e.Delta > 0)
						AdjustZoom(.1f);
					else
						AdjustZoom(-.1f);
				}
				SetStateOfControls();
				return;
			}
		}
		#endregion Mouse Functions

		#region Keyboard Functions
		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			int al = drawArea.TheLayers.ActiveLayerIndex;
			switch (e.KeyCode)
			{
				case Keys.Delete:
					drawArea.TheLayers[al].Graphics.DeleteSelection();
					drawArea.Invalidate();
					break;
				case Keys.Right:
					drawArea.PanX -= 10;
					drawArea.Invalidate();
					break;
				case Keys.Left:
					drawArea.PanX += 10;
					drawArea.Invalidate();
					break;
				case Keys.Up:
					if (e.KeyCode == Keys.Up &&
						e.Shift)
						AdjustZoom(.1f);
					else
						drawArea.PanY += 10;
					drawArea.Invalidate();
					break;
				case Keys.Down:
					if (e.KeyCode == Keys.Down &&
						e.Shift)
						AdjustZoom(-.1f);
					else
						drawArea.PanY -= 10;
					drawArea.Invalidate();
					break;
				case Keys.ControlKey:
					_controlKey = true;
					break;
				default:
					break;
			}
			drawArea.Invalidate();
			SetStateOfControls();
		}

		private void MainForm_KeyUp(object sender, KeyEventArgs e)
		{
			_controlKey = false;
		}
		#endregion Keyboard Functions

		#region Zoom, Pan, Rotation Functions
		/// <summary>
		/// Adjust the zoom by the amount given, within reason
		/// </summary>
		/// <param name="_amount">float value to adjust zoom by - may be positive or negative</param>
		private void AdjustZoom(float _amount)
		{
			drawArea.Zoom += _amount;
			if (drawArea.Zoom < .1f)
				drawArea.Zoom = .1f;
			if (drawArea.Zoom > 10)
				drawArea.Zoom = 10f;
			drawArea.Invalidate();
			SetStateOfControls();
		}

		private void tsbZoomIn_Click(object sender, EventArgs e)
		{
			AdjustZoom(.1f);
		}

		private void tsbZoomOut_Click(object sender, EventArgs e)
		{
			AdjustZoom(-.1f);
		}

		private void tsbZoomReset_Click(object sender, EventArgs e)
		{
			drawArea.Zoom = 1.0f;
			drawArea.Invalidate();
		}

		private void tsbRotateRight_Click(object sender, EventArgs e)
		{
			int al = drawArea.TheLayers.ActiveLayerIndex;
			if (drawArea.TheLayers[al].Graphics.SelectionCount > 0)
				RotateObject(10);
			else
				RotateDrawing(10);
		}

		private void tsbRotateLeft_Click(object sender, EventArgs e)
		{
			int al = drawArea.TheLayers.ActiveLayerIndex;
			if (drawArea.TheLayers[al].Graphics.SelectionCount > 0)
				RotateObject(-10);
			else
				RotateDrawing(-10);
		}

		private void tsbRotateReset_Click(object sender, EventArgs e)
		{
			int al = drawArea.TheLayers.ActiveLayerIndex;
			if (drawArea.TheLayers[al].Graphics.SelectionCount > 0)
				RotateObject(0);
			else
				RotateDrawing(0);
		}

		/// <summary>
		/// Rotate the selected Object(s)
		/// </summary>
		/// <param name="p">Amount to rotate. Negative is Left, Positive is Right, Zero indicates Reset to zero</param>
		private void RotateObject(int p)
		{
			int al = drawArea.TheLayers.ActiveLayerIndex;
			for (int i = 0; i < drawArea.TheLayers[al].Graphics.Count; i++)
			{
				if (drawArea.TheLayers[al].Graphics[i].Selected)
					if (p == 0)
						drawArea.TheLayers[al].Graphics[i].Rotation = 0;
					else
						drawArea.TheLayers[al].Graphics[i].Rotation += p;
			}
			drawArea.Invalidate();
			SetStateOfControls();
		}

		/// <summary>
		/// Rotate the entire drawing
		/// </summary>
		/// <param name="p">Amount to rotate. Negative is Left, Positive is Right, Zero indicates Reset to zero</param>
		private void RotateDrawing(int p)
		{
			if (p == 0)
				drawArea.Rotation = 0;
			else
			{
				drawArea.Rotation += p;
				if (p < 0) // Left Rotation
				{
					if (drawArea.Rotation <
						-360)
						drawArea.Rotation = 0;
				} else
				{
					if (drawArea.Rotation > 360)
						drawArea.Rotation = 0;
				}
			}
			drawArea.Invalidate();
			SetStateOfControls();
		}

		private void tsbPanMode_Click(object sender, EventArgs e)
		{
			_panMode = !_panMode;
			if (_panMode)
				tsbPanMode.Checked = true;
			else
				tsbPanMode.Checked = false;
			drawArea.ActiveTool = DrawArea.DrawToolType.Pointer;
			drawArea.Panning = _panMode;
		}

		private void tsbPanReset_Click(object sender, EventArgs e)
		{
			_panMode = false;
			if (tsbPanMode.Checked)
				tsbPanMode.Checked = false;
			drawArea.Panning = false;
			drawArea.PanX = 0;
			drawArea.PanY = drawArea.OriginalPanY;
			drawArea.Invalidate();
		}
		#endregion  Zoom, Pan, Rotation Functions

		private void tslCurrentLayer_Click(object sender, EventArgs e)
		{
			LayerDialog ld = new LayerDialog(drawArea.TheLayers);
			ld.ShowDialog();
			// First add any new layers
			for (int i = 0; i < ld.layerList.Count; i++)
			{
				if (ld.layerList[i].LayerNew)
				{
					Layer layer = new Layer();
					layer.LayerName = ld.layerList[i].LayerName;
					layer.Graphics = new GraphicsList();
					drawArea.TheLayers.Add(layer);
				}
			}
      drawArea.TheLayers.InactivateAllLayers();
			for (int i = 0; i < ld.layerList.Count; i++)
			{
        if (ld.layerList[i].LayerActive)
					drawArea.TheLayers.SetActiveLayer(i);

				if (ld.layerList[i].LayerVisible)
					drawArea.TheLayers.MakeLayerVisible(i);
				else
					drawArea.TheLayers.MakeLayerInvisible(i);

				drawArea.TheLayers[i].LayerName = ld.layerList[i].LayerName;
			}
			// Lastly, remove any deleted layers
			for (int i = 0; i < ld.layerList.Count; i++)
			{
				if (ld.layerList[i].LayerDeleted)
					drawArea.TheLayers.RemoveLayer(i);
			}
			drawArea.Invalidate();
		}

		#region Additional Drawing Tools
		/// <summary>
		/// Draw PolyLine objects - a polyline is a series of straight lines of various lengths connected at their end points.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsbPolyLine_Click(object sender, EventArgs e)
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.PolyLine;
			drawArea.DrawFilled = false;
		}

		private void tsbConnector_Click(object sender, EventArgs e)
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.Connector;
			drawArea.DrawFilled = false;
		}
		/// <summary>
		/// Draw Text objects
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tsbDrawText_Click(object sender, EventArgs e)
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.Text;
		}

		private void tsbFilledRectangle_Click(object sender, EventArgs e)
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.Rectangle;
			drawArea.DrawFilled = true;
		}

		private void tsbFilledEllipse_Click(object sender, EventArgs e)
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.Ellipse;
			drawArea.DrawFilled = true;
		}

		private void tsbImage_Click(object sender, EventArgs e)
		{
			drawArea.ActiveTool = DrawArea.DrawToolType.Image;
		}

		private void tsbSelectLineColor_Click(object sender, EventArgs e)
		{
			dlgColor.AllowFullOpen = true;
			dlgColor.AnyColor = true;
			if (dlgColor.ShowDialog() ==
				DialogResult.OK)
			{
                drawArea.LineColor = Color.FromArgb(255, dlgColor.Color);
                tsbLineColor.BackColor = Color.FromArgb(255, dlgColor.Color);
			}
		}

		private void tsbSelectFillColor_Click(object sender, EventArgs e)
		{
			dlgColor.AllowFullOpen = true;
			dlgColor.AnyColor = true;
            if (dlgColor.ShowDialog() ==
                DialogResult.OK)
            {
                drawArea.FillColor = Color.FromArgb(255, dlgColor.Color);
                tsbFillColor.BackColor = Color.FromArgb(255, dlgColor.Color);
            }
		}

		private void tsbLineThinnest_Click(object sender, EventArgs e)
		{
			drawArea.LineWidth = -1;
		}

		private void tsbLineThin_Click(object sender, EventArgs e)
		{
			drawArea.LineWidth = 2;
		}

		private void tsbThickLine_Click(object sender, EventArgs e)
		{
			drawArea.LineWidth = 5;
		}

		private void tsbThickerLine_Click(object sender, EventArgs e)
		{
			drawArea.LineWidth = 10;
		}

		private void tsbThickestLine_Click(object sender, EventArgs e)
		{
			drawArea.LineWidth = 15;
		}
		#endregion Additional Drawing Tools

		private void toolStripMenuItemGenericPen_Click(object sender, EventArgs e)
		{
			drawArea.PenType = DrawingPens.PenType.Generic;
			drawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.Generic);
		}

		private void redToolStripMenuItem_Click(object sender, EventArgs e)
		{
			drawArea.PenType = DrawingPens.PenType.RedPen;
			drawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.RedPen);
		}

		private void blueToolStripMenuItem_Click(object sender, EventArgs e)
		{
			drawArea.PenType = DrawingPens.PenType.BluePen;
			drawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.BluePen);
		}

		private void greenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			drawArea.PenType = DrawingPens.PenType.GreenPen;
			drawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.GreenPen);
		}

		private void redDottedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			drawArea.PenType = DrawingPens.PenType.RedDottedPen;
			drawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.RedDottedPen);
		}

		private void redDotDashToolStripMenuItem_Click(object sender, EventArgs e)
		{
			drawArea.PenType = DrawingPens.PenType.RedDotDashPen;
			drawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.RedDotDashPen);
		}

		private void doubleLineToolStripMenuItem_Click(object sender, EventArgs e)
		{
			drawArea.PenType = DrawingPens.PenType.DoubleLinePen;
			drawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.DoubleLinePen);
		}

		private void dashedArrowLineToolStripMenuItem_Click(object sender, EventArgs e)
		{
			drawArea.PenType = DrawingPens.PenType.DashedArrowPen;
			drawArea.CurrentPen = DrawingPens.SetCurrentPen(DrawingPens.PenType.DashedArrowPen);
		}

		private void exportToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Bitmap b = new Bitmap(drawArea.Width, drawArea.Height);
			Graphics g = Graphics.FromImage(b);
			g.Clear(Color.White);
			drawArea.TheLayers.Draw(g);
			b.Save(@"c:\test.bmp", ImageFormat.Bmp);
			MessageBox.Show("save complete!");
			g.Dispose();
			b.Dispose();
		}

    private void cutToolStripMenuItem_Click(object sender, EventArgs e)
    {
      drawArea.CutObject();
    }

	}
}