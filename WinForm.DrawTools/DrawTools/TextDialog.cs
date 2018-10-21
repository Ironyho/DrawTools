using System;
using System.Drawing;
using System.Windows.Forms;

namespace DrawTools
{
	public partial class TextDialog : Form
	{
		public TextDialog()
		{
			InitializeComponent();
		}

		private string _text;

		public string TheText
		{
			get { return _text; }
			set { _text = value; }
		}

		private Font _font;

		public Font TheFont
		{
			get { return _font; }
			set { _font = value; }
		}

		private Color _color;

		public Color TheColor
		{
			get { return _color; }
			set { _color = value; }
		}

		private void TextDialog_Load(object sender, EventArgs e)
		{
			_color = Color.Black;
			_font = txtTheText.Font;
			_text = "";
		}

		private void btnFont_Click(object sender, EventArgs e)
		{
			dlgFont.AllowSimulations = true;
			dlgFont.AllowVectorFonts = true;
			dlgFont.AllowVerticalFonts = true;
			dlgFont.MaxSize = 200;
			dlgFont.MinSize = 4;
			dlgFont.ShowApply = false;
			dlgFont.ShowColor = true;
			dlgFont.ShowEffects = true;
			if (dlgFont.ShowDialog() ==
			    DialogResult.OK)
			{
				_font = dlgFont.Font;
				_color = dlgFont.Color;
				txtTheText.Font = _font;
				txtTheText.ForeColor = _color;
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			_text = txtTheText.Text;
		}
	}
}