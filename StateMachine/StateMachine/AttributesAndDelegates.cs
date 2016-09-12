using System;

namespace StateMachine
{
	/// <summary>Custon link delegate.</summary>
	public delegate bool Link();

	/// <summary>  State tag that marks the state as INITIAL. </summary>
	[AttributeUsage (AttributeTargets.Class, AllowMultiple = false)]
	public class InitialAttribute : Attribute{}

	/// <summary> The link is implemented in the named state and it points to its holder state </summary>
	[AttributeUsage (AttributeTargets.Method, AllowMultiple = false)]
	public class ReverseAttribute : Attribute{}

	/// <summary> The node marked with this attribute overrides another already declared state (inheritance probably)</summary>
	[AttributeUsage (AttributeTargets.Class, AllowMultiple = false)]
	public class OverrideAttribute : Attribute{}	

	/// <summary> Marks this method as a LINK. </summary>
	[AttributeUsage (AttributeTargets.Method, AllowMultiple = false)]
	public class LinkToAttribute : Attribute
	{
		public string name; 
		public LinkToAttribute(string name)	{	this.name = name;}
		public LinkToAttribute(Type name)	{	this.name = name.Name;}
	}
	
	/// <summary> Mark links to be hidden from the omnicast. </summary>
	[AttributeUsage (AttributeTargets.Method, AllowMultiple = false)]
	public class MaskAttribute : Attribute
	{
		public Type[] mask;
		public MaskAttribute(params Type[] mask)	{	this.mask = mask;}
	}

	/// <summary> Marks the state machine this state makes part. </summary>
	[AttributeUsage (AttributeTargets.Class, AllowMultiple = false)]
	public class StateFromAttribute : Attribute
	{
		public string name;
		public StateFromAttribute(string name)	{	this.name = name;}
	}

	/// <summary>
	/// Link does not belong to current state machine exception.
	/// </summary>
	public class LinkNotBelongToStateMachineException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:LinkNotBelongToStateMachineException"/> class
		/// </summary>
		public LinkNotBelongToStateMachineException (string message) : base(message){}
	}

	/// <summary>
	/// Link with all and reverse attributes exception.
	/// </summary>
	public class LinkWithAllAndReverseAttributesException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:LinkWithAllAndReverseAttributesException"/> class
		/// </summary>
		public LinkWithAllAndReverseAttributesException (string message) : base(message){}
	}

	/// <summary>
	/// Missing initial node exception.
	/// </summary>
	public class MissingInitialStateException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MissingInitialStateException"/> class
		/// </summary>
		public MissingInitialStateException (string message) : base(message){}
	}

	/// <summary>
	/// Missing [node from] attribute exception.
	/// </summary>
	public class MissingStateFromAttributeException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MissingStateFromAttributeException"/> class
		/// </summary>
		public MissingStateFromAttributeException (string message) : base(message){}
	}

	/// <summary>
	/// Null state machine exception.
	/// </summary>
	public class NullStateMachineException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:NullStateMachineException"/> class
		/// </summary>
		public NullStateMachineException (string message) : base(message){}
	}

	/// <summary>
	/// Missing node exception.
	/// </summary>
	public class MissingStateException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MissingStateException"/> class
		/// </summary>
		public MissingStateException (string message) : base(message){}
	}

	/// <summary>
	/// Duplicated node name exception.
	/// </summary>
	public class DuplicatedStateNameException : Exception
	{		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:DuplicatedStateNameException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		public DuplicatedStateNameException (string message) : base (message){}
	}
}