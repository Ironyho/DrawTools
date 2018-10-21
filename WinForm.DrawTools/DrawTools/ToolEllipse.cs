using System.Drawing;
using System.Windows.Forms;

namespace DrawTools
{
	/// <summary>
	/// Ellipse tool
	/// </summary>
	internal class ToolEllipse : ToolRectangle
	{
		public ToolEllipse()
		{
			Cursor = new Cursor(GetType(), "Ellipse.cur");
		}

		public override void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
		{
			Point p = drawArea.BackTrackMouse(new Point(e.X, e.Y));
			if (drawArea.CurrentPen == null)
				AddNewObject(drawArea, new DrawEllipse(p.X, p.Y, 1, 1, drawArea.LineColor, drawArea.FillColor, drawArea.DrawFilled, drawArea.LineWidth));
			else
				AddNewObject(drawArea, new DrawEllipse(p.X, p.Y, 1, 1, drawArea.PenType, drawArea.FillColor, drawArea.DrawFilled));
		}
	}
}