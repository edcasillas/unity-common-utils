var commonUtils_webGL_logLevel = {
    None: 0,
    Debug: 1,
    Warning: 2,
    Error: 4
};

var commonUtils_webGL_verbosity = commonUtils_webGL_logLevel.Warning | commonUtils_webGL_logLevel.Error;

function commonUtils_webGL_log(message, logLevel) {
    // Assign default logLevel if not provided
    if (logLevel === undefined) { logLevel = commonUtils_webGL_logLevel.Debug; }

    // Early return if verbosity is set to None
    if (commonUtils_webGL_verbosity === commonUtils_webGL_logLevel.None) return;

    // Check if the logLevel is included in the verbosity
    if ((commonUtils_webGL_verbosity & logLevel) === logLevel) {
        if (logLevel === commonUtils_webGL_logLevel.Debug) {
            console.log('[CommonUtils:WebGLBridge-native] ' + message);
        } else if (logLevel === commonUtils_webGL_logLevel.Warning) {
            console.warn('[CommonUtils:WebGLBridge-native] ' + message);
        } else if (logLevel === commonUtils_webGL_logLevel.Error) {
            console.error('[CommonUtils:WebGLBridge-native] ' + message);
        }
    }
}
