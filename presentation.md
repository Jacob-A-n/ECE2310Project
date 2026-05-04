# Project idea

This is a windows event planner app that uses stnadard windows forms (winforms) library to display an interactive calendar, track events, create stopwatch and timers, add and manage notes.

This project uses main concepts of OOP such as inheritance, polymorphism and composition. 

# OOP principles in action

### Inheritance:
The RecurringEvent class inherits directly from the CalendarEvent base class (Event.cs), meaning it gets all the event features like titles, dates, etc and adds specific logic for repeating events. Also the main UI form inherits from System.Windows.Forms.Form the built in Windows framework

### Polymorphism: 
Classes like DrawCalendar and StudentNote use polymorphism by overriding the default ToString() method (inherited from the base object class) to output a customized, clean text format. It also uses method overloading (having multiple versions of the same method that accept different arguments) in constructors and in methods like UpdateNotificationTime().

### Composition & Aggregation: 
The Form class is composed of other objects. It holds a DrawCalendar to compute math, and lists (List<T>) of CalendarEvent, RecurringEvent, and StudentNote objects. It is the "brain" that pulls all these standalone objects together to display them to the user.