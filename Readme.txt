How to use:
Double left click anywhere on the screen to make a new dialog step

Dialog step:
	- this initially contains a single textbox, a colored header, 
a blue circle in the bottom left, and a black square in the bottom center
	
	- left clicking the black square will create a line centered at the square and anchored to your mouse.
This line indicate what the following dialog step will be. left click on the header of a diffrent dialog step to attach the other end of the line.
A single square can not have more than one line coming from it, but more than one square may connect to a single dialog step.
	
	- left clicking the blue circle will create a dialog option. These indicate what the player may respond.
Upon making a new option a darker textbox will apear right below the main textbox and a black square apear to the right side of this new box.
This new black square acts in exactly the same way as the one mentioned before. You may add as many options as you wish, each comes with their own black square.

	- you may move the dialog steps as desired simply by clicking and dragging on the header (note this feature is slightly bugged and may not be completely responsive)

	- in the upper right hand corner in the header there is a small checkbox. upon clicking this will turn the header red indicating that this will terminate the chat sequence
	
Buttons:
	Export:
		This button will export an xml file for use in other programs
	Import:
		This button will import an xml file and recreate the chat tree based on it.
	Note, this program will only recognize xml files that were generated from here, or in the same style as such.
	Do not try to use with random xml files.
	
	Save:
		This buttons saves your progess. In the future this will not do any checks and just keeps everything as is so you can come back to it later.
	This will export a .gz file. This files should not be shared as the NPC dialog script, use the Export button for that.
	
	Load:
		Loads from a .gz file to restore progress