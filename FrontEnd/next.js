// header
fetch("header.html")
  .then((response) => response.text())
  .then(
    (data) => (document.getElementById("header-placeholder").innerHTML = data)
  );

// note
var notification = document.getElementById("notification");
var pdf = document.getElementById("pdf");
var next_note = document.getElementById("next_note");
var prev_note = document.getElementById("prev_note");
var formde = document.getElementById("f-de");
function toggleElements(
  showNotification,
  showPrevNote,
  newFormdeWidth,
  newPdfWidth,
  newNotificationWidth
) {
  notification.style.display = showNotification ? "block" : "none";
  prev_note.style.display = showPrevNote ? "block" : "none";
  next_note.style.display = showPrevNote ? "none" : "block";

  setTimeout(function () {
    formde.style.width = newFormdeWidth;
    pdf.style.width = newPdfWidth;
    if (newNotificationWidth !== undefined) {
      notification.style.width = newNotificationWidth;
    }
  }, 10);
}

var currentWidth = window.getComputedStyle(formde).width;
// next_note
next_note.addEventListener("click", function () {
  if (currentWidth === "720px") {
    toggleElements(true, true, "720px", "400px", "280px");
  } else {
    toggleElements(true, true, "1020px", "610px", "370px");
  }
});

// prev_note
prev_note.addEventListener("click", function () {
  if (currentWidth === "720px") {
    toggleElements(false, false, "720px", "640px");
  } else {
    toggleElements(false, false, "905px", "840px");
  }
});
