# Description #

The `net/Db4objects.Db4o.WindowsService` project is a simple wrapper for db4o that hosts a db4o server instance. It is meant to be used as a starting point for customizing your own application service layer.

# Features #

  * Uses the default db4o network implementation without encryption. Replace this with your own network provider.
  * Add a reference to your class library. A Db4o ObjectServer needs to analyze your class implementation to ensure that some queries run as quickly as possible.

# Usage #

See the included [readme.pdf](http://code.google.com/p/db4o-extras/source/browse/trunk/net/Db4objects.Db4o.WindowsService/readme.pdf) for installation and usage instructions.