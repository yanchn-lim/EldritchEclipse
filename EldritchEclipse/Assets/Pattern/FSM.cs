using System.Collections;
using System.Collections.Generic;

namespace PGGE
{
	namespace Patterns
	{
		public class FSMState
		{
			public FSMState PreviousState { get; set; } = null;

			protected FSM mFsm;
			protected int mId;
			/* The constructor of the FSMState class
             * will require the parent FSM.
             * So we create a constructor 
             * with an instance of the FSM
             */
			public int ID { get { return mId; } }
			public FSMState()
			{
				mId = -1;
			}
			public FSMState(FSM fsm)
			{
				mId = -1;
				mFsm = fsm;
			}
			public FSMState(FSM fsm, int id)
			{
				mId = id;
				mFsm = fsm;
			}

			/*!Virtual method for entry to the state.
             * This method is called whenever this 
             * state is entered. Derived classes
             * must implemed this method and 
             * handle appropriately.
             */
			public virtual void Enter() { }

			/*!Virtual method for exit from the state.
             * This method is called whenever this 
             * state is exited. Derived classes
             * must implemed this method and 
             * handle appropriately.
             */
			public virtual void Exit() { }

			/*!Virtual method that will be 
             * called in every Update call from Unity.
             * The call will be routed via the
             * FSM through the current state.
             */
			public virtual void Update() { }

			/*!Virtual method that will be 
             * called in every FixedUpdate call from 
             * Unity. The call will be routed via the
             * FSM through the current state.
             */
			public virtual void FixedUpdate() { }
		}

		public class FSM
		{
			//A container object to store the set of states.
			//We will use a Dictionary container class to 
			//store the key, value pair of the set of states.
			//The key will be a unique ID for an
			//application-specific FSMState instance.

			protected Dictionary<int, FSMState> m_states = new Dictionary<int, FSMState>();

			public Dictionary<int, FSMState> States { get { return m_states; } }

			//!The current state that the FSM is in right now.
			protected FSMState m_currentState;

			public FSM()
			{
			}

			public void Add(FSMState state)
			{
				m_states.Add(state.ID, state);
			}

			public void Add(int key, FSMState state)
			{
				m_states.Add(key, state);
			}

			public FSMState GetState(int key)
			{
				return m_states[key];
			}

			public void SetCurrentState(int key)
			{
				SetCurrentState(GetState(key));
			}

			public FSMState GetCurrentState()
			{
				return m_currentState;
			}

			public void SetCurrentState(FSMState state)
			{
				if (m_currentState != null)
				{
					/* This from theory is the second code path. 
                     * The first code path is if the 
                     * m_currentState == null in which case
                     * nothing happens. 
                     * If the previous current state is
                     * valid then we will call the 
                     * Exit method of the previous
                     * current state
                     */
					m_currentState.Exit();
					state.PreviousState = m_currentState;
				}

				m_currentState = state;

				if (m_currentState != null)
				{
					/* We are now entering into a new FSMState
                     * So we will call the Enter method
                     * of the new current state.
                     */
					m_currentState.Enter();
				}
			}

			public void Update()
			{
				if (m_currentState != null)
				{
					m_currentState.Update();
				}
			}

			public void FixedUpdate()
			{
				if (m_currentState != null)
				{
					m_currentState.FixedUpdate();
				}
			}
		}
	}
}