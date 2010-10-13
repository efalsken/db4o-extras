import com.db4o.io.MemoryStorage

server.file='../database.db4o'
server.port=8080
// format is {username:passowrd}
server.users=[sa:"sa"]

// the server.config gives you native access to the db4o configuration API
server.config.file().blockSize(8)

server.config.file().storage(new MemoryStorage());