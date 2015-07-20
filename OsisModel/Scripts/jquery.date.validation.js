$(function () {
    $.validator.methods.date = function (value, element) {
        Globalize.culture("en-GB");
        // you can alternatively pass the culture to parseDate instead of
        // setting the culture above, like so:
        // parseDate(value, null, "en-AU")
        return this.optional(element) || Globalize.parseDate(value) !== null;
    }
});