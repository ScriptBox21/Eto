using Eto.Drawing;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eto.Designer
{
	public class DesignPanel : Scrollable, IDesignHost
	{
		DesignSurface designSurface;
		IInterfaceBuilder interfaceBuilder;

		public Action ContainerChanged { get; set; }
		public Action ControlCreating { get; set; }
		public Action ControlCreated { get; set; }
		public Action<DesignError> Error { get; set; }

		public BuilderInfo Builder { get; private set; }

		public DesignPanel()
		{
			designSurface = new DesignSurface();
			designSurface.InvalidateContent += (sender, e) => Update(_code);
			Border = BorderType.None;
			BackgroundColor = Global.Theme.DesignerBackground;
			Content = designSurface;
		}

		public Control GetContainer()
		{
			return this;
		}

		public string MainAssembly { get; set; }
		public IEnumerable<string> References { get; set; }

		IBuildToken token;
		string _code;

		public virtual void Update(string code)
		{
			if (interfaceBuilder == null)
				return;

			token?.Cancel();
			try
			{
				_code = code;
				token = interfaceBuilder.Create(_code, MainAssembly, References, ControlCreatedInternal, ErrorInternal);
			}
			catch (Exception ex)
			{
				ErrorInternal(ex);
			}
		}

		public static Control GetContent(Control content)
		{
			if (content is Window window)
			{
				// mac defaults to 200, 200.. why..
				var size = Platform.Instance.IsMac ? new Size(-1, -1) : window.ClientSize;
				// some platforms report 0,0 even though it probably should be -1, -1 initially.
				if (size.Width == 0)
					size.Width = -1;
				if (size.Height == 0)
					size.Height = -1;
				content = window.Content;
				window.Content = null;
				// swap out window for a panel so we can add it as a child
				content = new Panel
				{
					BackgroundColor = window.BackgroundColor,
					Padding = window.Padding,
					StyleProvider = window.StyleProvider,
					MinimumSize = window.MinimumSize,
					Size = size,
					Content = content
				};
			}
			else
			{
				content = new Panel
				{
					BackgroundColor = Global.Theme.DesignerPanel,
					Content = content
				};
			}
			return content;
		}

#pragma warning disable 414
		Control contentControl;
#pragma warning restore 414

		void ControlCreatedInternal(Control control)
		{
			try
			{
				ControlCreating?.Invoke();
				var old = contentControl;
				contentControl = control;
				designSurface.Content = GetContent(control);
				old?.Dispose();
				token = null;
				ControlCreated?.Invoke();
			}
			catch (Exception ex)
			{
				designSurface.Content = null;
				ErrorInternal(ex);
			}
		}

		void ErrorInternal(Exception ex)
		{
			// don't wipe clean, keep old state alive
			//designSurface.Content = null; 
			ex = ex.GetBaseException();
			Error?.Invoke(new DesignError { Message = ex.Message, Details = ex.ToString() });
		}

		public bool SetBuilder(string fileName)
		{
			Builder = BuilderInfo.Find(fileName);
			interfaceBuilder = Builder?.CreateBuilder();
			return Builder != null;
		}

		public string GetCodeFile(string fileName)
		{
			return Builder?.GetCodeFile(fileName);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				token?.Cancel();
			}
			base.Dispose(disposing);
		}
	}
}
