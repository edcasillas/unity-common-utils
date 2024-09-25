mergeInto(LibraryManager.library, {

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