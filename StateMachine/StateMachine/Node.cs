using System.Collections.Generic;
using System;

namespace StateMachine
{
	public abstract class State
	{
		/// <summary>
		/// Lista de links do nodo, contem um delegate com o metodo q verifica o link e o nodo q ele aponta
		/// </summary>
		public Dictionary<Link,State> links; 

		/// <summary>
		/// O Monobehaviour da pessoa q invoco a maquina de estados
		/// </summary>
		public Object m;

		/// <summary>
		/// Metodo que discreve o que acontece quando o nodo eh construdo.
		/// </summary>
		public virtual void Awake(){}

		/// <summary>
		/// Metodo q discreve o que acontece quando o nodo começa
		/// </summary>
		public virtual void Start(){}

		/// <summary>
		/// Metodo q discreve o update do nodo
		/// </summary>
		public virtual void Update(){}

		/// <summary>
		/// Metodo q discreve o que acontece quando o nodo sai
		/// </summary>
		public virtual void End(){}

	}
}
