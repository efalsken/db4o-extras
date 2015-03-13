# Getting Started #

To get started, download the [Java branch](https://db4o-extras.googlecode.com/svn/trunk/java) from source control. There are two directories. In the 'server' directory contains a db4o Java server. The 'extra' directory contains the other extras.

## Getting Started With The Server ##
The server is build using [Apache Ant](http://ant.apache.org/). You can call the 'ant'-command in the server directory. The result is in the target/dist directory.

The distribution has this layout:

  * dist/ All the db4o server service
    * bin/ The install / start / stop scripts
    * conf/ The configuration of the server
    * datamodel/ Drop you jars with the datamodel in this directory. It will be used by the server
    * lib/ The db4o & supporting jars
    * log/ The directory which the logs are written to

Run the server by executing the db4o-server install / start / stop commands. On Windows you need to execute the commands in an admin console. On Linux and MacOSX you need to execute the command with root-priviliges.



## Getting Started With The Extras ##
The extras are build using [Apache Maven](http://maven.apache.org/). You can call mvn package to build the project.

Or you can open the project you're favorite IDE, using a maven plugin.