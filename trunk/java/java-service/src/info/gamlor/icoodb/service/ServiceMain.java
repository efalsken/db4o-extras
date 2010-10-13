package info.gamlor.icoodb.service;

import info.gamlor.icoodb.service.configure.ConfigurationReader;

import java.io.IOException;
import java.util.logging.LogManager;


/**
 * @author roman.stoffel@gamlor.info
 * @since 1.09.2010
 */
public class ServiceMain
{
    private static final Object waitUntilExit = new Object();
    public static void main(String[] args) throws IOException {

        Configuration config = ConfigurationReader.create().configure();
        new Db4oService().start(config);
        waitUntiExit();
    }

    private static void waitUntiExit() {
        synchronized (waitUntilExit){
            try {
                waitUntilExit.wait();
            } catch (InterruptedException e) {
                Thread.currentThread().interrupt();
            }
        }
    }

}
