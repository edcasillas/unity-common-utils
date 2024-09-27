mergeInto(LibraryManager.library, {

    commonUtils_webGL_setupPointerLockEvents: function(gameObjectNamePtr) {
        // Convert the string pointer to the actual string
        var gameObjectName = UTF8ToString(gameObjectNamePtr);
        console.log('[CommonUtils] Setting up pointer lock events to game object ' + gameObjectName);
        var canvas = document.querySelector("#unity-canvas");

        function lockChangeAlert() {
            if (document.pointerLockElement === canvas || document.mozPointerLockElement === canvas) {
                // Call the Unity method 'OnPointerLockChanged' in the 'WebGLBridge' object
                //SendMessage('WebGLBridge', 'OnPointerLockChanged', 1);
                console.log('Will call pointer locked on game object ' + gameObjectName);
                SendMessage(gameObjectName, 'OnPointerLockChanged', 1);
                console.log('Pointer LOCKED');
            } else {
                //SendMessage('WebGLBridge', 'OnPointerLockChanged', 0);
                console.log('Will call pointer unlocked on game object ' + gameObjectName);
                SendMessage(gameObjectName, 'OnPointerLockChanged', 0);
                console.log('Pointer UNLOCKED');
            }
        }

        document.addEventListener('pointerlockchange', lockChangeAlert, false);
        document.addEventListener('mozpointerlockchange', lockChangeAlert, false);
    },

    commonUtils_webGL_removePointerLockEvents: function() {
        console.log('[CommonUtils] Removing pointer lock events.');
        var canvas = document.querySelector("#unity-canvas");

        // Check if the function reference exists before attempting to remove
        if (window.lockChangeAlert) {
            document.removeEventListener('pointerlockchange', window.lockChangeAlert, false);
            document.removeEventListener('mozpointerlockchange', window.lockChangeAlert, false);
            console.log('Pointer lock events unsubscribed');
        } else {
            console.log('No pointer lock events found to unsubscribe');
        }
    },

  commonUtils_webGL_goFullScreen: function () {
    if(instance) {
        instance.SetFullscreen(1);
    } else {
        console.log("Cannot go full screen because Unity Instance could not be found.");
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