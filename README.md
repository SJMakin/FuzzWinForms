# FuzzWinForms
##Random UI Fuzzer (Monkey-Fuzzer) for finding bugs in Windows Forms

####Features:

Random input of x number of actions using User32 api

Each input has a configurable chance of occuring
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

Interuption detection, so it doesnt take control over your session

#### TO DO:

* Add a log file reader class and add it to data collected during the test
* Add a db of previously seen error
* Further automate the workflow
* Add something to remove any datetimes from collected errors (else the replay (minimizer) wont match the errors - false negative)
