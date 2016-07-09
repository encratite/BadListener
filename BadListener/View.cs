using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BadListener.Error;

namespace BadListener
{
	public abstract class View<TModel>
        where TModel : class
	{
		private StringBuilder _StringBuilder = new StringBuilder();

		private List<Section> _Sections = new List<Section>();

		private string _Body = null;

		protected TModel Model { get; set; }

        protected string Layout { get; set; }

		protected dynamic ViewBag { get; set; } = new dynamic();

        public string Render(TModel model)
        {
            Model = model;
			Execute();
			string body = _StringBuilder.ToString();
			var layout = GetLayoutView();
			if (layout == null)
				return body;
			string content = layout.RenderLayout(body, _Sections, ViewBag);
			return content;
        }

		protected abstract void Execute();

		protected void Write(string literal)
		{
			_StringBuilder.Append(literal);
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
            string typeName = Layout ?? $"{typeof(TModel).Namespace}.Layout";
            var type = Type.GetType(typeName);
            if (type == null)
                return null;
            var instance = (View<object>)Activator.CreateInstance(type);
            return instance;
        }

		private string RenderLayout(string body, List<Section> sections, dynamic viewBag)
		{
			_Body = body;
			_Sections = sections;
			ViewBag = viewBag;
			Execute();
			string content = _StringBuilder.ToString();
			return content;
		}
	}
}
