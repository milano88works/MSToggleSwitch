using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace milano88.UI.Controls
{
    [DefaultEvent("CheckedChanged")]

    public class MSToggleSwitch : Control
    {
        public event EventHandler CheckedChanged;
        private BufferedGraphics _bufGraphics;


        public MSToggleSwitch()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            Size = new Size(40, 20);
            BackColor = Color.Transparent;
            UpdateGraphicsBuffer();
        }

        private void UpdateGraphicsBuffer()
        {
            if (Width > 0 && Height > 0)
            {
                BufferedGraphicsContext context = BufferedGraphicsManager.Current;
                context.MaximumBuffer = new Size(Width + 1, Height + 1);
                _bufGraphics = context.Allocate(CreateGraphics(), ClientRectangle);
            }
        }

        private Color onBackColor = Color.DodgerBlue;
        private Color onToggleColor = Color.White;
        private Color offBackColor = Color.DarkGray;
        private Color offToggleColor = Color.White;
        private bool _checked = false;

        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "DodgerBlue")]
        public Color OnBackColor
        {
            get { return onBackColor; }
            set
            {
                onBackColor = value;
                this.Invalidate();
            }
        }

        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "White")]
        public Color OnToggleColor
        {
            get { return onToggleColor; }
            set
            {
                onToggleColor = value;
                this.Invalidate();
            }
        }

        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "DarkGray")]
        public Color OffBackColor
        {
            get { return offBackColor; }
            set
            {
                offBackColor = value;
                this.Invalidate();
            }
        }

        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "White")]
        public Color OffToggleColor
        {
            get { return offToggleColor; }
            set
            {
                offToggleColor = value;
                this.Invalidate();
            }
        }

        public enum Style { Default, Square, iOS, Solid }
        Style _style;
        [Category("Custom Properties")]
        [DefaultValue(Style.Default)]
        public Style SwitchStyle
        {
            get { return _style; }
            set
            {
                _style = value;
                this.Invalidate();
            }
        }

        [Category("Custom Properties")]
        [DefaultValue(typeof(bool), "False")]
        public bool Checked
        {
            get => _checked;
            set
            {
                _checked = value;
                Invalidate();
                CheckedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void FillRoundedRect(Graphics graphics, RectangleF rect, Color color, float radius, bool solid = false)
        {
            using (GraphicsPath path = new GraphicsPath())
            using (SolidBrush brush = new SolidBrush(color))
            using (Pen pen = new Pen(Color.FromArgb(180, color)))
            using (Pen penSolid = new Pen(color, 2))
            {
                if (!solid)
                {
                    path.StartFigure();
                    path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
                    path.AddArc(rect.X + rect.Width - (radius * 2) - 1, rect.Y, radius * 2, radius * 2, 270, 90);
                    path.AddArc(rect.X + rect.Width - (radius * 2) - 1, rect.Y + rect.Height - (radius * 2) - 1, radius * 2, radius * 2, 0, 90);
                    path.AddArc(rect.X, rect.Y + rect.Height - (radius * 2) - 1, radius * 2, radius * 2, 90, 90);
                    path.CloseFigure();
                    graphics.DrawPath(pen, path);
                    graphics.FillPath(brush, path);
                }
                else
                {
                    path.StartFigure();
                    path.AddArc(rect.X + 0.5f, rect.Y + 0.5f, radius * 2, radius * 2, 180, 90);
                    path.AddArc(rect.X + rect.Width - (radius * 2) - 1 - 0.5f, rect.Y + 0.5f, radius * 2, radius * 2, 270, 90);
                    path.AddArc(rect.X + rect.Width - (radius * 2) - 1 - 0.5f, rect.Y + rect.Height - (radius * 2) - 1 - 0.5f, radius * 2, radius * 2, 0, 90);
                    path.AddArc(rect.X + 0.5f, rect.Y + rect.Height - (radius * 2) - 1 - 0.5f, radius * 2, radius * 2, 90, 90);
                    path.CloseFigure();
                    graphics.DrawPath(pen, path);
                    graphics.DrawPath(penSolid, path);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            float toggleSize = 12;
            _bufGraphics.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            switch (_style)
            {
                case Style.Default:
                    FillRoundedRect(_bufGraphics.Graphics, ClientRectangle, _checked ? onBackColor : offBackColor, 9);
                    FillRoundedRect(_bufGraphics.Graphics, new RectangleF(_checked ? Width - toggleSize - 4 : 4, (Height - toggleSize) / 2, toggleSize, toggleSize), _checked ? onToggleColor : offToggleColor, (toggleSize / 2) - 0.5F);
                    break;
                case Style.Square:
                    toggleSize = 16;
                    FillRoundedRect(_bufGraphics.Graphics, ClientRectangle, _checked ? onBackColor : offBackColor, 2);
                    FillRoundedRect(_bufGraphics.Graphics, new RectangleF(_checked ? Width - toggleSize - 2 : 2, (Height - toggleSize) / 2, toggleSize, toggleSize), _checked ? onToggleColor : offToggleColor, 1.5F);
                    break;
                case Style.iOS:
                    toggleSize = 16;
                    FillRoundedRect(_bufGraphics.Graphics, ClientRectangle, _checked ? onBackColor : offBackColor, 9);
                    FillRoundedRect(_bufGraphics.Graphics, new RectangleF(_checked ? Width - toggleSize - 2 : 2, (Height - toggleSize) / 2, toggleSize, toggleSize), _checked ? onToggleColor : offToggleColor, (toggleSize / 2) - 0.5F);
                    break;
                case Style.Solid:
                    toggleSize = 10;
                    if (!_checked)
                    {
                        FillRoundedRect(_bufGraphics.Graphics, ClientRectangle, offBackColor, 9, true);
                        FillRoundedRect(_bufGraphics.Graphics, new RectangleF(6, (Height - toggleSize) / 2, toggleSize, toggleSize), offBackColor, (toggleSize / 2) - 0.5F);
                    }
                    else
                    {
                        FillRoundedRect(_bufGraphics.Graphics, ClientRectangle, onBackColor, 9);
                        FillRoundedRect(_bufGraphics.Graphics, new RectangleF(Width - toggleSize - 6, (Height - toggleSize) / 2, toggleSize, toggleSize), onToggleColor, (toggleSize / 2) - 0.5F);
                    }
                    break;
            }

            _bufGraphics.Render(pevent.Graphics);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (Parent != null && BackColor == Color.Transparent)
            {
                Rectangle rect = new Rectangle(Left, Top, Width, Height);
                _bufGraphics.Graphics.TranslateTransform(-rect.X, -rect.Y);
                try
                {
                    using (PaintEventArgs pea = new PaintEventArgs(_bufGraphics.Graphics, rect))
                    {
                        pea.Graphics.SetClip(rect);
                        InvokePaintBackground(Parent, pea);
                        InvokePaint(Parent, pea);
                    }
                }
                finally
                {
                    _bufGraphics.Graphics.TranslateTransform(rect.X, rect.Y);
                }
            }
            else
            {
                using (SolidBrush backColor = new SolidBrush(BackColor))
                    _bufGraphics.Graphics.FillRectangle(backColor, ClientRectangle);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (Width < 40) Width = 40;
            Height = 20;
            UpdateGraphicsBuffer();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
                _checked = !_checked ? true : false;
            this.Invalidate();
            CheckedChanged?.Invoke(this, EventArgs.Empty);
        }

        [Browsable(false)]
        public override Image BackgroundImage { get => base.BackgroundImage; set { } }
        [Browsable(false)]
        public override ImageLayout BackgroundImageLayout { get => base.BackgroundImageLayout; set { } }
        [Browsable(false)]
        public override Font Font { get => base.Font; set { } }
        [Browsable(false)]
        public override string Text { get => base.Text; set { } }
        [Browsable(false)]
        public override Color ForeColor { get => base.ForeColor; set { } }
    }
}
