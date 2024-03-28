// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

<script>
    $(function () {
        $('#f_search').submit(function (e) {
            var title = $("#ipt_search").val().trim();
            if (title === "") {
                e.preventDefault();
                return false;
            }
        });
       });
</script>
