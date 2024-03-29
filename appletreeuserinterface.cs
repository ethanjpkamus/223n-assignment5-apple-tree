//Author: Ethan Kamus
//Email: ethanjpkamus@csu.fullerton.edu

/* The purpose of this program is to show the use of mouse click
 * events and how to change the animation of an "apple" whenever
 * it is clicked on. The goal is to have multiple apples on the
 * screen at once, but try with just one first
 *
 * This module specifically implements Form and contains the methods necessary
 * for this game to function properly
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Timers;

public class appletreeuserinterface : Form {

	//constants
	private const int MAXIMUM_FORM_WIDTH = 500;
	private const int MAXIMUM_FORM_HEIGHT = 800;
	private const int APPLE_RADIUS = 20;
	private const int GREENTOP = 500; //y value

	private static Random rnd = new Random();
	//variables
	private int apple_x = rnd.Next(MAXIMUM_FORM_WIDTH - 2*APPLE_RADIUS);
	private int apple_y = 0;
	private int applecenter_x = 0;
	private int applecenter_y = 0;
	private int mouse_x = 0;
	private int mouse_y = 0;
	private double distance = 0;
	private int apples_caught = 0;

	//items to be used in the user interface
	private Button start_button = new Button();
	private Button restart_button = new Button();
	private Button quit_button = new Button();

	private Label level_label = new Label();
	private Label apples_caught_label = new Label();

	private static System.Timers.Timer ui_clock = new System.Timers.Timer();
	private static System.Timers.Timer animation_clock = new System.Timers.Timer();

	//constructor
	public appletreeuserinterface(){

		applecenter_x = apple_x + APPLE_RADIUS;
		applecenter_y = apple_y + APPLE_RADIUS;

		MaximumSize = new Size(MAXIMUM_FORM_WIDTH,MAXIMUM_FORM_HEIGHT);
		MinimumSize = new Size(MAXIMUM_FORM_WIDTH,MAXIMUM_FORM_HEIGHT);

		Text = "Apple Tree Game by: Ethan Kamus";

		BackColor = Color.White;

		DoubleBuffered = true;

		ui_clock.Interval = 33.3; //30 Hz
		ui_clock.Enabled = true;
		ui_clock.AutoReset = true;
		ui_clock.Elapsed += new ElapsedEventHandler(manage_ui);

		animation_clock.Interval = 16.7; //60 Hz
		animation_clock.Enabled = false;
		animation_clock.AutoReset = true;
		animation_clock.Elapsed += new ElapsedEventHandler(manage_animation);

		start_button.Text = "START";
		start_button.Size = new Size(75,30);
		start_button.Location = new Point(10,GREENTOP+100);
		start_button.Click += new EventHandler(manage_start_button);

		restart_button.Text = "RESET";
		restart_button.Size = new Size(75,30);
		restart_button.Location = new Point(95,GREENTOP+100);
		restart_button.Click += new EventHandler(manage_restart_button);

		quit_button.Text = "QUIT";
		quit_button.Size = new Size(75,30);
		quit_button.Location = new Point(180,GREENTOP+100);
		quit_button.Click += new EventHandler(manage_quit_button);

		level_label.Text = "Level: 0";
		level_label.Size = new Size(75,30);
		level_label.ForeColor = Color.White;
		level_label.BackColor = Color.Green;
		level_label.Location = new Point(265,GREENTOP+100);

		apples_caught_label.Text = "Apples Caught: 0";
		apples_caught_label.Size = new Size(75,30);
		apples_caught_label.ForeColor = Color.White;
		apples_caught_label.BackColor = Color.Green;
		apples_caught_label.Location = new Point(265,GREENTOP+130);

		Controls.Add(start_button);
		Controls.Add(restart_button);
		Controls.Add(quit_button);
		Controls.Add(level_label);
		Controls.Add(apples_caught_label);


	} //end of constructor

	protected override void OnPaint(PaintEventArgs e){

		Graphics graph = e.Graphics;

		graph.FillRectangle(Brushes.Green,0,GREENTOP,500,300); //grass
		graph.FillRectangle(Brushes.Cyan,0,0,500,500); //sky

		if(animation_clock.Enabled){
		//only paint the ellipse if the clock is enabled
			graph.FillEllipse(Brushes.Red,apple_x,apple_y,APPLE_RADIUS*2,APPLE_RADIUS*2);
		}
		base.OnPaint(e);

		//check when the circle is above the grass and reset if it touches the ground
		if((apple_y + 2*APPLE_RADIUS) >= GREENTOP){
			//change level?
			apples_caught = 0;
			apples_caught_label.Text = "Apples Caught: 0";

			ResetApplePositions();
		}

		apples_caught_label.Text = "Apples Caught: " + apples_caught.ToString();

	} //end of OnPaint override

	protected override void OnMouseDown(MouseEventArgs e){
		mouse_x = e.X;
		mouse_y = e.Y;

		int center_of_apple_x = applecenter_x;
		int center_of_apple_y = applecenter_y;

		/* TODO: Find the distance from the center of the circle to the mouse click.
		 *	  If the value for the calculated distance is greater than the radius,
		 *	  then the click was not within the circle
		 */
		distance = Math.Sqrt( Math.Pow(mouse_x - center_of_apple_x,2)+
					 Math.Pow(mouse_y - center_of_apple_y,2) );

		//checks if the circle was above the green border
	  	//and if it was within the circle.
		if( center_of_apple_y > (GREENTOP + APPLE_RADIUS) && distance <= APPLE_RADIUS ){

			apples_caught++;

			//apples_caught_label.Text = "Apples Caught: " + apples_caught.ToString();

			//make new x pos for apple
			ResetApplePositions();
		}

	} //end of OnMouseDown override

	protected void manage_ui(Object o, ElapsedEventArgs e){

		Invalidate();

	} //end of manage_ui

	protected void manage_animation(Object o, ElapsedEventArgs e){

		//increment the position of the apple each time the clock ticks
		apple_y++;
		applecenter_y++;

		//check if the ball has touched the bottom of the screen.
		if((applecenter_y + APPLE_RADIUS) == MAXIMUM_FORM_HEIGHT){

			//move the apple back to the beginning
			ResetApplePositions();
			applecenter_x = apple_x + APPLE_RADIUS;
			applecenter_y = apple_y + APPLE_RADIUS;

		}

	} //end of manage_animation

	protected void manage_start_button(Object o, EventArgs e){

		animation_clock.Enabled = !animation_clock.Enabled;
		if(animation_clock.Enabled){
			start_button.Text = "Pause";
		}
		start_button.Text = "Resume";
	} //end of update_start_button

	protected void manage_restart_button(Object o, EventArgs e){

		//stop the animation
		animation_clock.Enabled = false;

		//reset the position of the apple
		ResetApplePositions();

		apples_caught = 0;
		apples_caught_label.Text = "Apples Caught: 0";

	} //end of update_restart_button

	protected void manage_quit_button(Object o, EventArgs e){

		Close();

	} //end of update_quit_button

	private void ResetApplePositions(){

		apple_x = rnd.Next(MAXIMUM_FORM_WIDTH - 2*APPLE_RADIUS);
		apple_y = 0;

	} //end of ResetApplePositions


} //end of ricochetballuserinterface implementation
