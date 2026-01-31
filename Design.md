# Game Design

## Premise

You are a plague doctor that wears a mask against the bad odors that makes all people sick in town. Your job is to count the dead and keep the rest of the town alive.

## Game Over States

When the game is done the player is shown how well they did.

### Win
- The player has found all people in town.

### Lose
- The player loses all hitpoints.

## World

Town that is populated by people

### People

People have hitpoints. They decrease every day by 25%.

People are in two states:
- Sick
  - Can be healed by the player, so they at least survive one day
- Deseased
  - Need to be accounted

People have names and are displayed in a list where there state is accounted for:
- **Unchecked** - the character was not seen yet
- **Dead** - the character died
- **Alive** - the character is alive
- **Unknown** - the character was alive yesterday

## Player Character

- Hitpoints decrease when Mask is empty

### Movement
- Regular Movement
- Running

### Mask
- Hitpoints

#### Reduction
- Reduced when player moves
- 2.5x reduced when player runs
- Reduced when healing a person
  
#### Increase
- Can be refilled with pouch 1x
- Can be refilled at refill stations

### Items
- Heal items can be used to heal people in town
