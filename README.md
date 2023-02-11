I was lately amazed by:\
https://oskarstalberg.com/game/wave/wave.html \
https://github.com/mxgmn/WaveFunctionCollapse

So i decided to do my own procedural generator. This project is **NOT** using any wave function collapse!\
The main reason is i just wanted a quick working test which i would like to port to esp32 + ILI9341.\
But since there is no real debug capability in arduino based esp32 projects this project was born.\
\
The main idea is that it generates only bottom to top left to right, and basically just rolls random numbers to place tiles.\
From all the available tiles first i filter out the ones that would fit for the next tile, e.g. the next tile is dependent on the previously placed one, therefore the continuality is preserved.\
\
The tiles are using a 2x2 'connection' matrix, where i tell the program which tiles are compatible with which.\
for example a tile:\
<img src="TileTest\tiles\wall_L.png"
     alt="Left Wall"
     width="100"
     style="float: left; margin-right: 10px;" />\
has a symmetry of:\
`WALL, SKY`\
`WALL, SKY`\
\
therefore on the right it can have only a tile which has compatible left side:\
`SKY, ...`\
`SKY, ...`
