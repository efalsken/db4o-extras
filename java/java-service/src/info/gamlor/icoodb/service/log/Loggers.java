package info.gamlor.icoodb.service.log;


import java.util.logging.Logger;

/**
 * @author roman.stoffel@gamlor.info
 * @since 11.09.2010
 */
public final class Loggers {
    private Loggers(){

    }

    public static Logger logger(Class forClass){
        return Logger.getLogger(forClass.getName());
    }
}
