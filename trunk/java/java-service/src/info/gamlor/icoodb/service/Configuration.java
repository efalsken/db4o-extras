package info.gamlor.icoodb.service;

import com.db4o.User;
import com.db4o.cs.Db4oClientServer;
import com.db4o.cs.config.ServerConfiguration;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * @author roman.stoffel@gamlor.info
 * @since 11.09.2010
 */
public class Configuration {
    private String file = "database.db4o";
    private int port = 8080;
    private Map<String,String> users = new HashMap<String, String>();
    private final ServerConfiguration config = Db4oClientServer.newServerConfiguration();

    public Configuration() {
        users.put("sa","sa");   
    }

    public String getFile() {
        return file;
    }

    public void setFile(String file) {
        this.file = file;
    }

    public int getPort() {
        return port;
    }

    public void setPort(int port) {
        this.port = port;
    }

    public Map<String, String> getUsers() {
        return users;
    }

    public void setUsers(Map<String, String> users) {
        this.users = users;
    }

    public List<User> users(){
        List<User> result = new ArrayList<User>();
        for (Map.Entry<String, String> entry : users.entrySet()) {
            result.add(new User(entry.getKey(),entry.getValue()));
        }
        return result;
    }

    public ServerConfiguration getConfig() {
        return config;
    }
}
