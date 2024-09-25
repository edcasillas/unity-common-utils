mergeInto(LibraryManager.library, {

  goFullScreen: function () {
    if(instance) {
        instance.SetFullscreen(1);
    } else {
        console.log("Cannot go full screen because Unity Instance could not be found.");
    }
  },

  isMobileBrowser: function () {
    return /iPhone|iPad|iPod|Android/i.test(navigator.userAgent);
  },

});