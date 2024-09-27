mergeInto(LibraryManager.library, {
    // Function to set verbosity from Unity
    commonUtils_webGL_setVerbosity: function(verbosity) {
        commonUtils_webGL_verbosity = verbosity;
        commonUtils_webGL_log('Verbosity set to: ' + verbosity)
    },

    commonUtils_webGL_setupPointerLockEvents: function(gameObjectNamePtr) {
        // Convert the string pointer to the actual string
        var gameObjectName = UTF8ToString(gameObjectNamePtr);
        commonUtils_webGL_log('Setting up pointer lock events to game object ' + gameObjectName);
        var canvas = document.querySelector("#unity-canvas");
// etc...
        function lockChangeAlert() {
            if (document.pointerLockElement === canvas || document.mozPointerLockElement === canvas) {
                // Call the Unity method 'OnPointerLockChanged' in the 'WebGLBridge' object
                commonUtils_webGL_log('Pointer LOCKED Will call pointer locked on game object ' + gameObjectName);
                SendMessage(gameObjectName, 'OnPointerLockChanged', 1);
            } else {
                //SendMessage('WebGLBridge', 'OnPointerLockChanged', 0);
                commonUtils_webGL_log('Pointer UNLOCKED Will call pointer unlocked on game object ' + gameObjectName);
                SendMessage(gameObjectName, 'OnPointerLockChanged', 0);
            }
        }

        document.addEventListener('pointerlockchange', lockChangeAlert, false);
        document.addEventListener('mozpointerlockchange', lockChangeAlert, false);
    },

    commonUtils_webGL_removePointerLockEvents: function() {
        commonUtils_webGL_log('Removing pointer lock events.');
        var canvas = document.querySelector("#unity-canvas");

        // Check if the function reference exists before attempting to remove
        if (window.lockChangeAlert) {
            document.removeEventListener('pointerlockchange', window.lockChangeAlert, false);
            document.removeEventListener('mozpointerlockchange', window.lockChangeAlert, false);
            commonUtils_webGL_log('Pointer lock events unsubscribed');
        } else {
            commonUtils_webGL_log('No pointer lock events found to unsubscribe', commonUtils_webGL_logLevel.Warning);
        }
    },

    // Function to disable default behavior for a specific key
    commonUtils_webGL_disableDefaultBehaviorForKey: function(keyNamePtr) {
        var key = UTF8ToString(keyNamePtr);
        commonUtils_webGL_log('Disabling default behavior for key: ' + key);

        // Add keydown event listener
        window.addEventListener('keydown', function(e) {
            if (e.key === key) {
                commonUtils_webGL_log('Preventing default behavior for key: ' + key);
                e.preventDefault();  // Prevent the default behavior when the key is pressed
            }
        }, false);
    },

  commonUtils_webGL_goFullScreen: function () {
        // TODO This will only work if instance is set in the template!
    if(instance) {
        instance.SetFullscreen(1);
    } else {
        commonUtils_webGL_log("Cannot go full screen because Unity Instance could not be found.", commonUtils_webGL_logLevel.Error);
    }
  },

  commonUtils_webGL_isMobileBrowser: function () {
    return /iPhone|iPad|iPod|Android/i.test(navigator.userAgent);
  },

  commonUtils_webGL_getUserAgent: function() {
    var userAgent = navigator.userAgent;
    return allocate(intArrayFromString(userAgent), 'i8', ALLOC_NORMAL);
  }

});