using System;       
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using BadListener.Error;

namespace BadListener
{
	public abstract class View<TModel>
		where TModel : class
	{
		private StringBuilder _StringBuilder;

        private string _Body;

		private List<Section> _Sections;

		protected TModel Model { get; set; }

        protected dynamic ViewBag { get; set; }

		protected string Layout { get; set; }

		public string Render(TModel model)
		{
            _StringBuilder = new StringBuilder();
            _Sections = new List<Section>();
            Model = model;
            ViewBag = new ExpandoObject();
			Execute();
			string body = _StringBuilder.ToString();
			var layout = GetLayoutView();
            string output = body;
			if (layout != null)
            {
                _StringBuilder = new StringBuilder();
			    string content = layout.RenderLayout(_StringBuilder, body, _Sections, ViewBag);
                output = content;
            }
            Cleanup();
            return output;
		}

		protected abstract void Execute();

		protected void Write(object literal)
		{
            if (literal != null)
			    _StringBuilder.Append(literal.ToString());
		}

		protected void RenderBody()
		{
			if (_Body == null)
				throw new ViewError("No body has been defined.");
			Write(_Body);
		}

		protected void DefineSection(string name, Action render)
		{
			if (_Sections.Any(s => s.Name == name))
				throw new ViewError($"Section \"{name}\" had already been defined.");
			var section = new Section(name, render);
			_Sections.Add(section);
		}

		protected void RenderSection(string name)
		{
			var section = _Sections.FirstOrDefault(s => s.Name == name);
			if (section == null)
				throw new ViewError($"Unable to find section \"{name}\"");
			section.Render();
		}

		private View<object> GetLayoutView()
		{
            var viewType = GetType();
			string typeName = Layout ?? $"{viewType.Namespace}.Layout";
			var type = viewType.Assembly.GetType(typeName);
			if (type == null)
				return null;
			var instance = (View<object>)Activator.CreateInstance(type);
			return instance;
		}

		private string RenderLayout(StringBuilder stringBuilder, string body, List<Section> sections, dynamic viewBag)
		{
            _StringBuilder = stringBuilder;
			_Body = body;
			_Sections = sections;
			ViewBag = viewBag;
			Execute();
			string content = _StringBuilder.ToString();
            Cleanup();
			return content;
		}

        private void Cleanup()
        {
            _StringBuilder = null;
            _Sections = null;
            Model = null;
            ViewBag = null;
        }
	}
}
