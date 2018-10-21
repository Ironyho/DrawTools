using System.Collections.Generic;

namespace DrawTools
{
	/// <summary>
	/// Delete command
	/// </summary>
	internal class CommandDelete : Command
	{
		private List<DrawObject> cloneList; // contains selected items which are deleted

		// Create this command BEFORE applying Delete All function.
		public CommandDelete(Layers list)
		{
			cloneList = new List<DrawObject>();

			// Make clone of the list selection.

			foreach (DrawObject o in list[list.ActiveLayerIndex].Graphics.Selection)
			{
				cloneList.Add(o.Clone());
			}
		}

		public override void Undo(Layers list)
		{
			list[list.ActiveLayerIndex].Graphics.UnselectAll();

			// Add all objects from cloneList to list.
			foreach (DrawObject o in cloneList)
			{
				list[list.ActiveLayerIndex].Graphics.Add(o);
			}
		}

		public override void Redo(Layers list)
		{
			// Delete from list all objects kept in cloneList

			int n = list[list.ActiveLayerIndex].Graphics.Count;

			for (int i = n - 1; i >= 0; i--)
			{
				bool toDelete = false;
				DrawObject objectToDelete = list[list.ActiveLayerIndex].Graphics[i];

				foreach (DrawObject o in cloneList)
				{
					if (objectToDelete.ID ==
					    o.ID)
					{
						toDelete = true;
						break;
					}
				}

				if (toDelete)
				{
					list[list.ActiveLayerIndex].Graphics.RemoveAt(i);
				}
			}
		}
	}
}