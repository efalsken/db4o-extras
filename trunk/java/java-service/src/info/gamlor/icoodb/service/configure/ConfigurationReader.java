package info.gamlor.icoodb.service.configure;

import info.gamlor.icoodb.service.CancelledServerStart;
import info.gamlor.icoodb.service.Configuration;

import javax.script.ScriptEngine;
import javax.script.ScriptEngineManager;
import javax.script.ScriptException;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.util.logging.Level;
import java.util.logging.Logger;

import static info.gamlor.icoodb.service.log.Loggers.logger;

/**
 * @author roman.stoffel@gamlor.info
 * @since 11.09.2010
 */
public class ConfigurationReader {
    private final static Logger log = logger(ConfigurationReader.class);
    private final ScriptEngine engine;
    public static final String CONFIG_FILE = "conf/db4o.groovy";

    private ConfigurationReader(ScriptEngine engine) {
        this.engine = engine;
    }

    public static ConfigurationReader create(){
        ScriptEngineManager factory = new ScriptEngineManager();
        ScriptEngine engine = factory.getEngineByName("groovy");
        return new ConfigurationReader(engine);
    }

    public Configuration configure(){
        log.info("Start reading configuration");
        Configuration config = new Configuration();
        engine.put("server",config);
        try {
            engine.eval(new FileReader(findOutPath()));
            log.info("Finished reading configuration");
            return config;
        } catch (ScriptException e) {
            log.log(Level.SEVERE,"Couldn't read configuration-file "+fullpath()+" "+e.getMessage(),e);
            throw new CancelledServerStart(e);
        } catch (FileNotFoundException e) {
            log.log(Level.SEVERE,"Couldn't find configuration-file "+fullpath()+" "+e.getMessage(),e);
            throw new CancelledServerStart(e);
        }
    }

    private String findOutPath() {
        String localDir = "./" + CONFIG_FILE;
        if(new File(localDir).exists()){
            return localDir;
        } else{
            return "../"+CONFIG_FILE;
        }
    }

    private String fullpath() {
        return new File(findOutPath()).getAbsolutePath();
    }
}
