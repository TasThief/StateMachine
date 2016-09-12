using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StateMachine
{
	public static class MagicDictionary
	{
		///Dictionary id, it got a state machine name and a who built it
		public class MagicDictionaryKeyID
		{
			/// <summary>
			/// The name of the state machine.
			/// </summary>
			public string name;
			
			/// <summary>
			/// The class who built this state machine.
			/// </summary>
			public Type baseCasterType;
			
			/// <summary>verify if this object is equal to another.</summary>
			/// <returns><c>true</c>, true if it is equal, <c>false</c> otherwise.</returns>
			public bool isEqual (MagicDictionaryKeyID obj)
			{	
				return (name == obj.name && baseCasterType == obj.baseCasterType);
			}
			
			/// <summary>
			/// Initializes a new instance of the magic dictionary id <see cref="StateMachine.Engine2+MagicDictionaryKeyID"/> class.
			/// </summary>
			public MagicDictionaryKeyID(string name, Type casterType)
			{
				this.name = name;
				baseCasterType = casterType;
			}
		}
		
		/// <summary>
		/// The magic dictionary. with all state machies architecture blueprints
		/// </summary>
		private static Dictionary<MagicDictionaryKeyID, List<StateID>> magicDictionary;
		
		/// <summary>Fills the magic dictionary link lists.</summary>
		/// <param name="id">Identifier in the magic dictionary to fill its nodes.</param>
		private static void FillMagicDictionaryLinkLists(MagicDictionaryKeyID pageId)
		{
			foreach (StateID state in magicDictionary[pageId]) 										//for each state in this dictionary's page
			{
				foreach (MethodInfo link in state.stateType.GetMethods())									//for each method this state has									
				{																							//save a list of string related to the link
					LinkToAttribute linkTo = (LinkToAttribute)Attribute.GetCustomAttribute(link,typeof(LinkToAttribute));

					if (linkTo!=null)																		//if that method uses the tag link
					{
						if (linkTo.name == "All")																//if that link uses the name all
						{
							if (link.GetCustomAttributes(typeof(ReverseAttribute),false).Length >= 1)				//if the link uses the tag reverse
								throw new LinkWithAllAndReverseAttributesException("Links do tipo All n podem ter o atributo [Reverse]");					//throw an error

							Type[] mask = new Type[0];															//initialize a linking mask

							if (state.stateType.GetCustomAttributes(typeof(MaskAttribute),false).Length>1)			//if the state is using a mask						
								mask = ((MaskAttribute)Attribute.GetCustomAttribute(link,typeof(MaskAttribute))).mask;	//save it in a list of mask strings
							
							foreach (StateID omniReceiverState in magicDictionary[pageId])   							//for each state in the page, implements a reverse link for the actual state
								if  (!mask.Contains(omniReceiverState.stateType))									//if the mask does not contains the current state
									omniReceiverState.LinkList.Add(new LinkID(link, state, state));				//implements a reverse state
						}

						else if (link.GetCustomAttributes(typeof(ReverseAttribute),false).Length >= 1)			//if the link utilizes the tag reverse
						{
							StateID auxStateId = SearchStateWithName(pageId, linkTo.name);
							auxStateId.LinkList.Add (new LinkID(link, state, state ));								//searchs for a state with the same name and implements a reverse links on that state to the actual state
						}
						else 																					//of if the link does not use any special tags
						{
							StateID auxStateId = SearchStateWithName(pageId,linkTo.name);
							state.LinkList.Add (new LinkID(link, auxStateId, state));	
						}
					}
				}
			}
		}
		
		/// <summary>Searchs the state with the given name </summary>
		/// <returns>The state with name.</returns>
		/// <param name="id">Identifier in the magic dictionary.</param><param name="name">Name to be search.</param>
		private static StateID SearchStateWithName (MagicDictionaryKeyID pageId, string name)
		{
			foreach (StateID state in magicDictionary[pageId])
				if (state.stateType.Name == name)
					return state;
			
			throw new MissingStateException(name+ " state does not exists..." );
		}
		
		/// <summary>Gets a page in the magic dictionary.</summary>
		/// <returns>The page in magic dictionary.</returns>
		/// <param name="id">Page identifier.</param>
		public static List<StateID> GetAPageInMagicDictionary(MagicDictionaryKeyID pageId)
		{
			List<StateID> stateList;										//reverence for the return variableo
			
			if (magicDictionary == null)								//if the dictionary does not exists
			{
				magicDictionary  =  new Dictionary<MagicDictionaryKeyID, List<StateID>>();		//creates it
				
				stateList = BuildMagicDictionaryPageContent(pageId);					//get the list of states and save it at the return variable
				
				AddAPageInMagicDictionary(pageId,stateList);						//put it on the dictionary
			}
			else if (!MagicDictionaryContains(pageId))						//if it exists but it's not on the dictionary
			{
				stateList = BuildMagicDictionaryPageContent(pageId);					//get a list of states and save's it to return later
				
				AddAPageInMagicDictionary(pageId,stateList);						//put it in the dicionary
			}
			else 														//if it exists in the dicationary
			{
				stateList = magicDictionary[pageId];								//prepare it to be returned
			}
			return stateList;											//return the found blueprint
		}
		
		/// <summary>Adds A page in the magic dictionary.</summary>
		/// <returns><c>true</c>, if A page in magic dictionary was added, <c>false</c> otherwise.</returns>
		/// <param name="id">Page identifier.</param> param name="stateList">Page content.</param>
		private static void AddAPageInMagicDictionary (MagicDictionaryKeyID pageId, List<StateID> stateList)
		{
			if (stateList.Count > 0)						//if the state list exists
			{
				magicDictionary.Add (pageId, stateList);			//add it to the dictionary
				
				FillMagicDictionaryLinkLists(pageId);			//fill the link lists inside the states
			}
			else 										//if the state list does not exist
				throw new NullStateMachineException("the state machine named " + pageId.name + " does not contain states... did u mispelled its name?"); 
		}
		
		///<summary> If the magics the dictionary contains certain . </summary>
		/// <returns><c>true</c>, if dictionary contains the page with the identifier, <c>false</c> otherwise.</returns>
		private static bool MagicDictionaryContains(MagicDictionaryKeyID soughtPage)
		{
			bool result = false;
			
			foreach (KeyValuePair<MagicDictionaryKeyID,List<StateID>> page in magicDictionary) 
				if (page.Key.isEqual(soughtPage))
					result = true;
			
			return result;
		}
		
		/// <summary>Builds a page content getting a reflection from caster's type and build an avaliable state list </summary>
		/// <returns>The state list.</returns>
		/// <param name="name">State machine name.</param> <param name="caster">User's caster.</param> <param name="initialNode">Initial state.</param> <typeparam name="T">User's type.</typeparam>
		private static List<StateID> BuildMagicDictionaryPageContent(MagicDictionaryKeyID pageId)
		{
			List<StateID> stateList = new List<StateID>();				//the list that will be returned
			List<StateID> overrideStateList = new List<StateID>();		//list with medthods that are blocking others
			bool initialStateNotFound = true;						//control variable that marks if the initial state was already found
			bool wasNotOverriden = true;							//control variable that marks if the state will be overloaded
			
			
			for (Type baseType = pageId.baseCasterType; baseType != null; baseType = baseType.BaseType)				//for each class in the hierarchy
			{
				foreach (Type state in baseType.GetNestedTypes(BindingFlags.Public))										//for each nested class
				{
					if (state.IsSubclassOf(typeof(State))) 																//verify if that class is a state
					{																										//save the attribute stateFrom for further validation
						StateFromAttribute attribute = (StateFromAttribute)Attribute.GetCustomAttribute(state,typeof(StateFromAttribute),false);
						
						if (attribute == null) 																				//verify if its not an attribute
						{
							throw new MissingStateFromAttributeException("State " + state.FullName + " does not have an identification tag [StateFrom(string name)]!!!");	//sends an error message
						}	
						else if (attribute.name == pageId.name)																	//verifies if the state has an identification to this state machine
						{
							wasNotOverriden = true;																				//sets the override control variable before starts the verificationo
							
							foreach (StateID overridenState in overrideStateList)													//for each state on the state list that are overriding others
								if (overridenState.stateType.Name == state.Name)														//if the state's name is in the bloked list
									wasNotOverriden = false;																			//set this state as overriden and jump the next steps
							
							if (wasNotOverriden)																					//if it was not overridend
							{
								if (state.GetCustomAttributes(typeof(OverrideAttribute),false).Length >= 1)								//if it has the tag override 
								{
									overrideStateList.Add (new StateID(state, baseType));														//gets in the override list marking it to override another state
									stateList.Add 		 (new StateID(state, baseType));														//it enters in a list of commum states
								}
								else if (initialStateNotFound && state.GetCustomAttributes(typeof(InitialAttribute),false).Length >= 1)	//if the initial state was not created, check if this state has the initial tag
								{
									stateList.Insert(0,new StateID(state, baseType));															//save the initial as the first state
									initialStateNotFound = false;																			//marks the initial state as found
								}
								else 																									//if it does not have any special tag
								{
									stateList.Add (new StateID(state, baseType));																//enters in the common state list
								}	
							}
						}
					}
				}
			}
			if (initialStateNotFound)																				//if the initial state was not found throws an error
				throw new MissingInitialStateException(pageId.name +"'s machine does not have a Initial attribute [Initial]");

			return stateList;
		}
		
	}
}
