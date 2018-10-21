using System;
using System.Drawing;
using System.Windows.Forms;

namespace DrawTools
{
	/// <summary>
	/// Image tool
	/// </summary>
	internal class ToolImage : ToolObject
	{
		public ToolImage()
		{
			Cursor = new Cursor(GetType(), "Rectangle.cur");
		}

		public override void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
		{
			Point p = drawArea.BackTrackMouse(new Point(e.X, e.Y));
			AddNewObject(drawArea, new DrawImage(p.X, p.Y));
		}

		public override void OnMouseMove(DrawArea drawArea, MouseEventArgs e)
		{
			drawArea.Cursor = Cursor;

			if (e.Button ==
			    MouseButtons.Left)
			{
				Point point = drawArea.BackTrackMouse(new Point(e.X, e.Y));
				int al = drawArea.TheLayers.ActiveLayerIndex;
				drawArea.TheLayers[al].Graphics[0].MoveHandleTo(point, 5);
				drawArea.Refresh();
			}
		}

		public override void OnMouseUp(DrawArea drawArea, MouseEventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Select an Image to insert into map";
			ofd.Filter = "Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|Fireworks (*.png)|*.png|GIF (*.gif)|*.gif|Icon (*.ico)|*.ico|All files|*.*";
			ofd.InitialDirectory = Environment.SpecialFolder.MyPictures.ToString();
			int al = drawArea.TheLayers.ActiveLayerIndex;
			if (ofd.ShowDialog() ==
			    DialogResult.OK)
				((DrawImage)drawArea.TheLayers[al].Graphics[0]).TheImage = (Bitmap)Bitmap.FromFile(ofd.FileName);
			ofd.Dispose();
			base.OnMouseUp(drawArea, e);
		}
	}
}