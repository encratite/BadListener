using System;

namespace BadListener
{
	public abstract class View<TModel>
        where TModel : class
	{
		protected TModel Model { get; set; }

        protected string Layout { get; set; }

        public void Render(TModel model = null)
        {
            Model = model;
            var layout = GetLayoutView();
            throw new NotImplementedException();
        }

		protected abstract void Execute();

		protected void Write(string literal)
		{
			Context.Writer.Write(literal);
		}

		protected void RenderBody()
		{
			throw new NotImplementedException();
		}

		protected void DefineSection(string name, Action body)
		{
			throw new NotImplementedException();
		}

		protected void RenderSection(string name)
		{
			throw new NotImplementedException();
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
	}
}
