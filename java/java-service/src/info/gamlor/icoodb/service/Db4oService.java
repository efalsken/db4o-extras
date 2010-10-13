package info.gamlor.icoodb.service;

import com.db4o.ObjectServer;
import com.db4o.User;
import com.db4o.cs.Db4oClientServer;

import java.util.logging.Logger;

import static info.gamlor.icoodb.service.log.Loggers.logger;

/**
 * @author roman.stoffel@gamlor.info
 * @since 1.09.2010
 */
class Db4oService {
    private final static Logger log = logger(Db4oService.class);
    private ObjectServer server;
    public void start(Configuration configuration){
        log.info("Starting db4o server");
        server = Db4oClientServer.openServer(configuration.getConfig(),
                configuration.getFile(),
                configuration.getPort());
        log.info("Finished starting db4o server");
        logInfo(configuration);
        addUsers(configuration, server);
    }


    private void logInfo(Configuration configuration) {
        log.info("Server is listening to port "+configuration.getPort());
        log.info("Opened database "+configuration.getFile());
    }

    private void addUsers(Configuration configuration, ObjectServer server) {
        for (User user : configuration.users()) {
            server.grantAccess(user.name,user.password);
            log.info("Added user "+user.name+" to the server ");
        }
    }
}
