# dsa-Bora0Dev
dsa-Bora0Dev created by GitHub Classroom
# COMMENTARY.md

## Data Structure Used

This project uses a **graph data structure** to represent the world map of the medieval market simulator. 
Each **town** is modeled as a node, while **roads** connecting towns are represented as edges that include toll costs. 
This structure was chosen because it efficiently models interconnected locations, allowing flexible movement between towns. 
The graph also supports future expansion — new towns or roads can easily be added without changing the overall system design. 
It’s an intuitive and scalable way to simulate a network of locations and travel costs.

## Player Choices and State Management

Player data and choices are stored in a combination of **variables** and **dictionaries**. 
The player’s **current location** is stored as a string, and **gold** is tracked as an integer. 
Inventory is represented as a dictionary that maps each good’s name to its quantity. 
Each town also maintains its own dictionary of goods and prices, allowing dynamic and independent market conditions. 
When the player buys or sells, the program updates both the player’s inventory and gold balance accordingly. 
Travel choices are managed by validating whether a destination town exists as an adjacent node in the graph, ensuring only valid paths are accessible.

## Challenges in Implementing Narrative Logic

One of the biggest challenges in implementing the narrative logic was maintaining consistency across player actions while keeping the game loop simple and readable. 
Because this is a text-based console game, all feedback to the player must be clearly conveyed through printed messages. 
Ensuring that every command — from `buy` to `travel` — responded correctly required careful handling of input parsing and state updates. 

Another difficulty was balancing simplicity and realism. 
I wanted the player to feel a sense of agency when trading and traveling, without overcomplicating the economy or movement system. 
This required designing a straightforward logic for tolls, prices, and inventory management while still maintaining meaningful decision-making. 

Lastly, managing the **flow of the narrative** without a graphical interface posed design challenges. 
The game’s text prompts needed to guide the player naturally through exploration and trade, which required testing various message formats and command responses. 
Overall, the final structure reflects an effective balance between clarity, interactivity, and maintainable code architecture.

## Gameplay

# https://youtu.be/TEk6C4YmE5g
