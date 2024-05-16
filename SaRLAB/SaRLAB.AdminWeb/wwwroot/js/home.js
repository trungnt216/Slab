$(document).ready(function () {
  $(".subject-content").children().hide();

  // Lấy trạng thái mục đã chọn từ localStorage
  var selectedId = localStorage.getItem("selectedSubject");
  var selectedItem = localStorage.getItem("selectedItem");

  if (selectedId) {
    // Nếu có mục đã chọn trước đó, đặt nó là active và hiển thị nội dung tương ứng
    $('a[data-id="' + selectedId + '"]').addClass("active");
    $(
      "#" +
        $('a[data-id="' + selectedId + '"]')
          .data("target")
          .substring(1)
    ).show();
  } else {
    // Nếu không có mục đã chọn trước đó, đặt mặc định là mục "Cấu hình" và hiển thị nội dung tương ứng
    $('a[data-id="config"]').addClass("active");
    $("#collapseConfig").show();
  }

  if (!selectedItem) {
    // Nếu không có mục nào được chọn trước đó, đặt mặc định là mục "Banner trang chủ"
    var activeItem = $('a.item[data-url="/Configuration/GetAllBanner"]');
    activeItem.addClass("active");

    activeItem.closest(".collapse").addClass("show");
    loadContent("/Configuration/GetAllBanner");
  } else {
    // Nếu đã có mục được chọn trước đó, thiết lập lại lớp active cho mục đó
    var activeItem = $('a.item[data-url="' + selectedItem + '"]');
    activeItem.addClass("active");

    activeItem.closest(".collapse").addClass("show");
    loadContent(selectedItem);
  }

  $(".subject a").on("click", function (e) {
    e.preventDefault(); // Ngăn chặn hành vi mặc định của thẻ <a>
    // Xóa class active từ tất cả các mục
    $(".subject a").removeClass("active");

    // Thêm class active vào mục được chọn
    $(this).addClass("active");
    var target = $(this).data("target");

    // Ẩn tất cả các nội dung và chỉ hiển thị nội dung tương ứng
    $(".subject-content").children().hide();
    $(target).show();

    // Lưu trạng thái mục được chọn vào localStorage
    localStorage.setItem("selectedSubject", $(this).data("id"));
  });

  $("a.item").on("click", function (event) {
    event.preventDefault();

    $("a.item").removeClass("active");
    $(this).addClass("active");

    var url = $(this).data("url");

    loadContent(url);

    localStorage.setItem("selectedItem", url);
  });

  function loadContent(url) {
    $.ajax({
      url: url,
      type: "GET",
      success: function (data) {
        var subContent = $(data).find("#sub-content").html();
        $("#content .home-container").html(subContent);
        history.pushState(null, "", url);
      },
      error: function (xhr, status, error) {
        console.log("Đã xảy ra lỗi: " + error);
      },
    });
  }

  $("#ellipsis-icon").on("click", function () {
    $("#dropdown-content").toggle();
  });

  $(document).on("click", function (e) {
    if (!$(e.target).closest(".dropdown").length) {
      $("#dropdown-content").hide();
    }
  });
});
