using System;
using System.Collections.Generic;

namespace StateMachine
{
	public class Engine
	{
		/// <summary>
		/// The name of this state machine.
		/// </summary>
		private string name;

		/// <summary>
		/// The current active node. 
		/// </summary>
		public State activeState;

		/// <summary>
		/// The last node before the active node becomes the active one.
		/// </summary>
		public State lastNode;

		/// <summary>
		/// All nodes listed by their names.
		/// </summary>
		public Dictionary<string,State> nodes;

		/// <summary>
		/// Builds a new instance of the Engine together with the whole state machine structure class.
		/// </summary>
		/// <param name="name">State machine name.</param>
		/// <param name="caster">Who owns the state machine.</param>
		public Engine(string name, System.Object caster)
		{
			this.name = name;

			List<StateID> blueprint = MagicDictionary.GetAPageInMagicDictionary(new MagicDictionary.MagicDictionaryKeyID(name,caster.GetType()));			//Get the statemachine blueprint and add to the dictionary

			Dictionary<StateID, State> nodeReferenceTable = new  Dictionary<StateID, State>();                              //Start a list with nodes and its build information

			nodes = new Dictionary<string, State>();

			State aux;																										//Auxiliar variable

			foreach (StateID nodeInfo in blueprint) 																			//Initialize states in the states reference's list
			{
				aux = (State)Activator.CreateInstance(nodeInfo.stateType);														//Create an instence of a state

				aux.m = caster;																									//passes the object that is building the state machine
				
				nodeReferenceTable.Add(nodeInfo, aux);																			//add the state at the reference table

				nodes.Add(nodeInfo.stateType.Name, aux);																			//save the state list for future consult
			}
	
			foreach (KeyValuePair<StateID,State> node in nodeReferenceTable)													//for each state in the reference list.
			{
				node.Value.links = new Dictionary<Link, State>();															//initialize a list of links for each state

				foreach (LinkID link in node.Key.LinkList) 																	//for each link inside each object in the state list.
					node.Value.links.Add((Link)Delegate.CreateDelegate(typeof(Link),nodeReferenceTable[link.caster],link.link),nodeReferenceTable[link.target]); //creates a page in the dictionary containing a delegate and a reference to another state
			}

			activeState = nodeReferenceTable[blueprint[0]];																	//stes the initial state as the first one on the blueprint
			lastNode = activeState;

			foreach (KeyValuePair<StateID,State> node in nodeReferenceTable)													//wakeup all states
				node.Value.Awake();

			activeState.Start();	 																							//runs the start method on the initial node
		}

		/// <summary>
		/// Sets the active state, running the end step from previous statee and the start step from the new one.
		/// </summary>
		/// <param name="newNode">New node.</param>
		public void SetActiveNode(State newNode)
		{
			string attName = ((StateFromAttribute)Attribute.GetCustomAttribute(newNode.GetType(),typeof(StateFromAttribute),false)).name;

			if (attName == name)
			{
				activeState.End();
				lastNode = activeState;
				activeState = newNode;
				activeState.Start();
			}
			else
			{
				throw new LinkNotBelongToStateMachineException("The machine " + name + " does not contain the state " + attName);
			}
		}

		/// <summary>
		/// Run this active state update and check if its links for a swap
		/// </summary>
		public void Run()
		{
			activeState.Update();
			CheckConditions();
		}

		/// <summary>
		/// Checks the active state conditions.
		/// </summary>
		private void CheckConditions()
		{
			foreach (var link in activeState.links) 
				if (link.Key())
				{
					SetActiveNode(link.Value);
					break;
				}
		}
	}
}