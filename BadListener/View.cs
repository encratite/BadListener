using System;

namespace BadListener
{
	public abstract class View<TModel>
	{
		protected TModel Model { get; set; }

		public abstract void Render();

		protected void Write(string literal)
		{
			Context.ResponseBuilder.Append(literal);
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
	}
}
