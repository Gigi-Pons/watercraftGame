# watercraftGame
This game consists of a watercraft that moves in different directions, up, down, left, and right, through the use of the arrows on the keyboard.
You can shoot balls using the space bar, these balls will eliminate the fish coming from the right side of the screen at random times and random location
on the Y axis.
There are three different types of fish, a long nose fish, an octopus, and a grey big teeth fish.  
These fish have different values and functions assigned to them, if not killed, those values will be subtracted from the total score, if killed, 
the values will be added to the score.

Find a link to the video [here](https://youtu.be/KLVkrzS69V0)

## Octopus
The octopus has a value of 30 points assigned to it.  Also, if the player (watercraft) is touched by one of these, it will grow twice in size for the next 3 seconds and will be a greater target for other fish.

## Long nose fish
The long nose fish has a value of 20 points assigned to it.  If the player is touched by on of these, it will decrease its speed by .5 for the next 3 seconds and will be a greater target because it can't move as fast as before.

## Big teeth fish
This fish has a value of 10 points assigned to it.  If the player is touched by this fish, nothing will happen.

## Break down of code
There are a total of 7 files with extension .cs.  The sprite class is a parent class and classes, UserControlledSprite, AutomatedSprite, and Projectile are children of this class.  SpriteManager is where the instantiation of objects of these children classes happens.  Here, there is an in depth implementation of methods: LoadContent, Update, and Draw.  In the Update method, 5 other methods are called.

  ### SpawnEnemy()
  This method takes care of the random position where the next enemy (fish) will be placed on the screen.  It also takes the likelihood of each one
  and creates a new fish accordingly.  If random number is between 0 and 33, an octopus will be created.  If random number is between 0 and 66, a big
  teeth fish will be created.  If random number is between 0 and 99, a long nose fish will be created.
  ### ResetSpawnTime()
  Sets a new spawn time between 1 and 2 seconds until next enemy is released.
  ### UpdateSprites()
  Checks if there has been a collision, if so, then act accordingly.  Meaning, subtract if fish not killed, and add score if killed.  As well as 
  disappear fish and ball from screen.  In this method, the creation of balls also takes place.  Each projectile object is instantiated here if space
  is pressed.
  ### AdjustSpawnTime()
  This method adjusts the difficulty of the game.  As the game progresses, more fish will be created which makes it harder to kill all of them and easier
  to accidentally start losing lives by touching them.  
  ### CheckPowerUpExpiration()
  This method checks when the 3 seconds are up for the octopus and long nose fish.  That way, the watercraft can go back to its regular size, and start
  moving at a normal speed again.
