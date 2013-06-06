using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace JeuDuMoulin
{
	class Human : Plateau, IPlayer
	{

		public Game.PlayerControl Control { get; private set; }
		private Guid currentToken;

		public StepAction CurrentAction { get; set; }

		public Human()
		{
			this.Player = this;
			InitializeComponent();
		}

		public void Initialize(Game game)
		{
			this.Game = game;
			this.Control = new Game.PlayerControl(this);
			this.CurrentAction = StepAction.None;
			this.Invalidate();

			//graphic debug
			//Random r = new Random(42);
			//foreach (var node in Game.Board.Nodes)
			//{
			//    node.Occupation = (Occupation)r.Next(3);
			//}
			//SelectedNode = Game.Board.Nodes.ElementAt(3);
		}

		private void Human_MouseClick(object sender, MouseEventArgs e)
		{
			if (!Game.TurnHandler.IsMyTurn(currentToken))
			{
				return;
			}
			var clickedNode = MapPointToNode(Game.Board, e.Location, Origin, SpacingCoef);
			if (clickedNode != null)
			{
#if DEBUG
				Logging.Log("clicked on " + clickedNode.Id.ToString());
#endif
				if (e.Button == System.Windows.Forms.MouseButtons.Left)
				{
					//normal selection
					if (clickedNode.Owner != null)
					{
						if (CurrentAction == StepAction.RemoveOpponentPawn)
						{
							if (clickedNode.Owner != null && clickedNode.Owner != this)
							{
								Control.RemoveOpponentPawn(currentToken, clickedNode);
								CurrentAction = StepAction.None;
								this.Invalidate();
								OnGraphicRefresh();
							}
						}
						if (CurrentAction == StepAction.MovePawnConstrained || CurrentAction == StepAction.MovePawnFreely)
						{
							if (SelectedNode != clickedNode)
							{
								if (clickedNode.Owner == this)
								{
									SelectedNode = clickedNode;
									this.Invalidate();
								}
							}
						}
					}
					else //empty node
					{
						if (CurrentAction == StepAction.PlacePawn)
						{
							Control.PlacePawn(currentToken, clickedNode);
							CurrentAction = StepAction.None;
							this.Invalidate();
							OnGraphicRefresh();
						}
					}
				}
				else if (e.Button == System.Windows.Forms.MouseButtons.Right)
				{
					if (CurrentAction == StepAction.MovePawnConstrained || CurrentAction == StepAction.MovePawnFreely)
					{
						//move the pawn
						if (SelectedNode != null &&
							clickedNode.Owner == null &&
							SelectedNode != clickedNode)
						{
							if (CurrentAction == StepAction.MovePawnConstrained)
							{
								if (SelectedNode.Neighbors.Contains(clickedNode))
								{
									Control.MovePawnConstrained(currentToken, SelectedNode, clickedNode);
									CurrentAction = StepAction.None;
									SelectedNode = null;
									this.Invalidate();
									OnGraphicRefresh();
								}
							}
							else if (CurrentAction == StepAction.MovePawnFreely)
							{
								Control.MovePawnFreely(currentToken, SelectedNode, clickedNode);
								CurrentAction = StepAction.None;
								SelectedNode = null;
								this.Invalidate();
								OnGraphicRefresh();
							}
						}
					}
				}
			}
			else
			{
#if DEBUG
				Logging.Log("clicked on nothing");
#endif
			}
		}

		public void PlacePawn(Guid token)
		{
			this.currentToken = token;
			this.Invalidate();
		}

		public void RemoveOpponentPawn(Guid token)
		{
			this.currentToken = token;
			this.Invalidate();
		}

		public void MovePawnConstrained(Guid token)
		{
			this.currentToken = token;
			this.Invalidate();
		}

		public void MovePawnFreely(Guid token)
		{
			this.currentToken = token;
			this.Invalidate();
		}

		public override string ToString()
		{
			return Game.Player1 == this ? "Player1" : "Player2";
		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// Human
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.Name = "Human";
			this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Human_MouseClick);
			this.ResumeLayout(false);

		}

	}
}
