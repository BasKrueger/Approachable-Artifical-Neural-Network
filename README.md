# Approachable Artifical Neural Network
<p align="center">
    <img src="readme/ANN_Banner_.png" alt="ANN Banner"><br>
</p>

"Approachable Artifical Neural Network" aims to drastically lower the barrier of entry for the Artifical Neural Network AI and NEAT learning algorithm within the context of Unity Game Development.
To achieve this goal I created my own implementation of the AI algorithm alongside wrapper classes, while supporting it with custom editor scripts and an exensive documentation.

This Project was created as my Thesis at the [S4G School for Games](https://www.school4games.net/).

# Overview
 <p align="left">
    <img src="readme/trained.gif" alt="trained Car"><br>
</p>

Artifical Neural Networks are a special kind of AI that work similar to human brains. This makes the AI very flexible, as that allows it to find gameplay patterns on its own. 
This asset boils down the usage of this algorithm to the following:
 1. What can the AI see? (in this case 5 different distances in front of them)
 2. What can the AI do? (in this case turn left, turn right and control the speed)
 3. When does the AI do something good/bad? (in this case it gets punished for touching the track boundary and rewarded based on how far it got)

And that's all you have to do. After you trained the AI you are able to save and manage them as simple scriptable object.
For more informations check out the full [Documentation](https://github.com/BasKrueger/Approachable-Artifical-Neural-Network/blob/main/Documentation.pdf), which also goes through the algorithm itself in full detail.

# How to run
Simply download the reposetory and open the "Project" folder using at least Unity 2022.3.

# Highlight editor extensions
 <p align="left">
    <img src="readme/Visualization.PNG" alt="Editor Visualization"><br>
</p>

Aside its own implementation of the artifical neural network algorithm, this project also makes extensive use of editor extensions. 
Those not only make it easier to use the algorithm, but it is also capable of visualizing any artifical neural network in real time using the Unity inspector.

