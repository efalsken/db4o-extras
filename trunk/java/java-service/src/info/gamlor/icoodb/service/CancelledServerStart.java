package info.gamlor.icoodb.service;

/**
 * @author roman.stoffel@gamlor.info
 * @since 11.09.2010
 */
public class CancelledServerStart extends RuntimeException {
    public CancelledServerStart(String message) {
        super(message);
    }

    public CancelledServerStart(String message, Throwable cause) {
        super(message, cause);
    }

    public CancelledServerStart(Throwable cause) {
        super(cause);
    }
}
