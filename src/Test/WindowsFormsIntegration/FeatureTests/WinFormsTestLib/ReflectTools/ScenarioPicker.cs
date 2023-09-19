// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using WFCTestLib.Log;
using WFCTestLib.Util;
using System.IO.IsolatedStorage;
using System.Xml;


//DESCRIPTION:  The ScenarioPicker Dialog displays all the scenarios for a given test and
//allows a user to select the ones that they want to run.  The /SCENARIOPICKER
//command line argument must be used for the dialog to be displayed.
namespace ReflectTools
{
	public partial class ScenarioPicker : Form
	{
		#region Constructor and Init

		private ArrayList _tests;			//This is the original list of ScenarioGroups
		private int _countdown;			//Int to decrement the countdown
		private string _testName;			//This is the name of the current test to use as the filename
		private Hashtable _allNodes;		//This HT stores all the nodes for easy lookup
		private ArrayList _selectedNodes;	//This ArrayList contains the HT Key for the selected nodes in m_allNodes

		//The constructor takes in all the scenarios and the test name.
		public ScenarioPicker(ArrayList tests, string testName)
		{
			InitializeComponent();

			_countdown = 3;				//set the countdown to 3 seconds
			_tests = tests;				//store off the original ScenarioGroups
			_testName = testName;			//store off the name of the test
			_allNodes = new Hashtable(100);//init the list of all nodes

			LoadScenarios(tests);			//add all the scenarios to the tree view
			GetSavedSelections();			//check any saved scenarios

			timer1.Enabled = true;			//start the countdown timer
		}

		#endregion 

		#region Properties

		//this property is called when the dialog is closed.  It returns a new
		//ArrayList for all the scenarios that were selected.
		public ArrayList SelectedScenarios
		{
			get
			{
				//count is used as the index for the hashtable of selected nodes
				//it will be added to the m_SelectedNodes ArrayList so that
				//if we decide to save the selections we can easily go back 
				//and get the nodes that were selected.
				int count = 0;
				_selectedNodes = new ArrayList();

				//ar is our new ArrayList with the selected ScenarioGroups and Scenarios
				ArrayList ar = new ArrayList();

				//The top leve of the tree nodes will contain the ScenarioGroups
				for (int i = 0; i < tvScenarios.Nodes.Count; i++)
				{
					count++;	//increment count for each node we encounter

					//If the ScenarioGroup is checked then loop through it's children to see
					//which specific scenarios have been checked
					if (tvScenarios.Nodes[i].Checked)
					{
						//Create a new ScenarioGroup and set it's name to the text of the
						//selected node
						ScenarioGroup sg = new ScenarioGroup();
						sg.Name = tvScenarios.Nodes[i].Text;
						
						_selectedNodes.Add(count); //store off the index of node

						//We use an arraylist because we don't know how big the actual
						//MethodInfo array shoudl be.  
						ArrayList tmp = new ArrayList();
						
						for(int j=0; j< tvScenarios.Nodes[i].Nodes.Count; j++)
						{
							count++; //increment count for each node we encounter

							//If the child is checked grab the MethodInfo object from
							//the nodes tag and add it to the tmp ArrayList.  Save off
							//the node index.
							if (tvScenarios.Nodes[i].Nodes[j].Checked)
							{
								tmp.Add(tvScenarios.Nodes[i].Nodes[j].Tag);
								_selectedNodes.Add(count);
							}
						}

						//Create the MethodInfo Array and add it to our ScenarioGroup
						MethodInfo[] scenarios = new MethodInfo[tmp.Count];
						tmp.CopyTo(scenarios);
						sg.Scenarios = scenarios;

						//Add the ScenarioGroup to the ArrayList to be returned
						ar.Add(sg);
					}					
				}
				return ar;
			}
		}

		#endregion 

		#region Control Events

		//if the Save Selections checkbox was checked, save the selections
		//otherwise clear the store, set the dialog re---- to OK and close
		private void btnOK_Click(object sender, EventArgs e)
		{
			if (chkPersist.Checked)
				SaveSelections();
			else
				ClearStore();

			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		//Uncheck all of the scenarios
		private void llUncheck_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			CheckAll(tvScenarios.Nodes, false);
		}

		//Check all of the scenarios
		private void llCheckAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			CheckAll(tvScenarios.Nodes, true);
		}

		//The timer event decrements the countdown and updates the status label.  If the countdown
		//passes 0 the dialog result is set to cancel and this dialog is closed
		private void timer1_Tick(object sender, EventArgs e)
		{
			_countdown--;
			if (_countdown < 0)
			{
				this.DialogResult = DialogResult.Cancel;
				this.Close();
			}
			else
			{
				tssLabel.Text = String.Format("Continuing in {0} seconds...", _countdown.ToString());
			}
		}

		//The override button allows a user to change the scenario selection.  First it disables
		//the timer, makes visible the other controls, enables the ListView and updates the status text.
		private void btnOverride_Click(object sender, EventArgs e)
		{
			timer1.Enabled = false;
			btnOverride.Visible = false;
			btnReset.Visible = true;
			btnOK.Visible = true;
			llUncheck.Visible = true;
			llCheckAll.Visible = true;
			tvScenarios.Enabled = true;
			tssLabel.Text = "Select the scenarios you want to run.";
			chkPersist.Visible = true;
		}

		//The reset button sets reverts back to the saved scenario selection
		private void btnReset_Click(object sender, EventArgs e)
		{
			GetSavedSelections();
		}

		//The AfterCheck event will uncheck all child nodes if the 
		//parent (aka ScenarioGroup) is unchecked since you cannot have a
		//Scenario without a ScenarioGroup
		private void tvScenarios_AfterCheck(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Parent == null)
				if (!e.Node.Checked)
					CheckAll(e.Node.Nodes, e.Node.Checked);
		}

		//The BeforeCheck event will make sure that a ScenarioNode cannot be checked
		//if it's parent (aka ScenaroGroup) is unchecked since you cannot have a
		//Scenario without a ScenarioGroup
		private void tvScenarios_BeforeCheck(object sender, TreeViewCancelEventArgs e)
		{
			if (e.Node.Parent != null)
				//We only want to cancle the check if this is a keyboard or mouse action since
				//other functions check/uncheck node; this would be an Unknown TreeViewAction
				if (e.Action == TreeViewAction.ByKeyboard || e.Action == TreeViewAction.ByMouse)
					e.Cancel = !e.Node.Parent.Checked;
		}

		#endregion

		#region Private Functions

		//The CheckAll function enumerates all the scenarios and sets the items checked state to whatever
		//is passed in
		private void CheckAll(TreeNodeCollection nodes, bool check)
		{			
			for (int i = 0; i < nodes.Count; i++)
			{
				nodes[i].Checked = check;

				if (nodes[i].Nodes.Count > 0)
				{
					CheckAll(nodes[i].Nodes, check);
				}
			}
		}

		//GetSavedSelections uses isolated storage to look for previously selected scenario's for this test.
		//The data store is a simple file named after the test.
		private void GetSavedSelections()
		{
			//First uncheck any previously checked nodes
			CheckAll(tvScenarios.Nodes, false); 

			IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
			StreamReader reader = new StreamReader(new IsolatedStorageFileStream(_testName, FileMode.OpenOrCreate, isoStore));
			try
			{
				//if the reader is empty just check all the nodes
				if (reader.EndOfStream)
				{
					CheckAll(tvScenarios.Nodes, true);
				}
				else 
				{
					//otherwise, loop through the file grabing the node index of the previously saved scenarios
					//and mark that node as checked
					while (!reader.EndOfStream)
					{
						CheckNode(reader.ReadLine());
					}

					//if we actualy found something turn on the SaveSelections check box
					chkPersist.Checked = true;
				}
			}
			catch
			{
				//if we get any kind of exception, simply just check all the nodes
				CheckAll(tvScenarios.Nodes, true);
			}
			finally
			{				
				reader.Close();
				isoStore.Close();
			}
		}

		//CheckNode takes in the node index that corrisponds to an item in the m_allNodes HT
		//if it found it marks that node as checked.
		private void CheckNode(string savedIndex)
		{
			if (savedIndex != "")
			{
				((TreeNode)_allNodes[Convert.ToInt32(savedIndex)]).Checked = true;
			}
		}

		//The SaveSelections function uses isolated storage to save a list of the selected scenarios.
		//each selection is stored as the index in which the node was added to the tree.  The name of the test
		//is used as the file name for storage.
		private void SaveSelections()
		{
			//First Clear out the store - delete the file.
			ClearStore();

			IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
			
			// Assign the writer to the store and the file TestStore.
			StreamWriter writer = new StreamWriter(new IsolatedStorageFileStream(_testName, FileMode.OpenOrCreate, isoStore));

			try
			{
				//calling selected Scenarios populates the m_SelectedNode hash table, we don't care about the returned ArrayList
				ArrayList ar = SelectedScenarios;
				for (int i = 0; i < _selectedNodes.Count; i++)
				{
					writer.WriteLine(_selectedNodes[i].ToString());
				}

				writer.Close();
				isoStore.Close();
			}
			catch
			{
				//If we get any kind of exception just clear out the store
				ClearStore();
			}
		}

		//The ClearStore function simply deletes the storage file if it exists.
		private void ClearStore()
		{
			IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

			string[] fileNames = isoStore.GetFileNames(_testName);

			foreach (string file in fileNames)
			{
				if (file == _testName)
				{
					isoStore.DeleteFile(_testName);
				}
			}

			isoStore.Close();
		}

		//this function just adds all the scenarios to the tree view
		private void LoadScenarios(ArrayList tests)
		{
			//First grab off the list of ScenarioGroups
			TreeNode[] items = new TreeNode[tests.Count];

			//nodeCount will be incremented so that we can uniquely identify 
			//the node in the hashtable.  
			int nodeCount = 0;  

			for (int i = 0; i < tests.Count; i++)
			{
				//Create our first ScenarioGroup and corrisponding TreeNode
				ScenarioGroup sg = tests[i] as ScenarioGroup;				
				TreeNode sgNode = new TreeNode(sg.Name);
				nodeCount++;  //increment nodeCount for each new TreeNode
			
				items[i] = sgNode;
				items[i].Checked = true;

				//Add the new node to the m_allNodes HT				
				_allNodes.Add(nodeCount, sgNode);

				//Next loop through all the scenarios for this Scenariogroup and add them
				//to the tree.
				MethodInfo[] scenarios = sg.Scenarios;				

				for (int j = 0; j < scenarios.Length; j++)
				{
					string description = String.Empty;
					string name = String.Empty;

					name = scenarios[j].Name;

					//check and see if the scenario has a description attribute, if so add it as well.
					object[] attrs = scenarios[j].GetCustomAttributes(typeof(ScenarioAttribute), true);
					
					if (attrs != null)
					{
						if (attrs.Length == 1)  // if there is more than one attribute then don't do anything
							description = ((ScenarioAttribute)attrs[0]).Description;
					}

					//Create the new node and store the MethodInfo object in the Node's Tag property
					TreeNode sNode = new TreeNode(name);
					nodeCount++;

					sNode.Checked = true;
					sNode.ToolTipText = description;
					sNode.Tag = scenarios[j];

					//Add the new node to the m_allNodes HT				
					_allNodes.Add(nodeCount, sNode);

					//Add the Scenario Node to the ScenarioGroup Node
					items[i].Nodes.Add(sNode);					
				}
			}

			//Add all the ScenarioGroups and ScenarioNodes to the Tree View
			tvScenarios.Nodes.AddRange(items);
		}

		#endregion



	}
}
