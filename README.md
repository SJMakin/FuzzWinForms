# FuzzWinForms
##Random UI Fuzzer (Monkey-Fuzzer) for finding bugs in Windows Forms

####Features:

Random input of x number of actions using User32 API

Each input has a configurable chance of occurring
  * Mouse click events
    - Left click
    - Right click
    - Middle Click
    - Click and Drag
  * Keyboard events
    - Most keys on keyboard with chance of modifiers being active (SHIFT/CTRL/ATL)
    - A dictionary of 'evil' strings which it may use

Action recording and replay functionality

Replay minimizer based on Lithium (most replays go down to <10 actions)

Application Event log capture with filters

Interruption detection, so it doesn't take control over your session

Pre/Post-Test actions, so you can (restore databases/config files/etc to) ensure a consistent test  

Distance comparison on errors, so the minimizer will work if the error doesnt match 100%

#### TO DO:

* Add a log file reader class and add it to data collected during the test
* Add a db of previously seen error
* Further automate the workflow

