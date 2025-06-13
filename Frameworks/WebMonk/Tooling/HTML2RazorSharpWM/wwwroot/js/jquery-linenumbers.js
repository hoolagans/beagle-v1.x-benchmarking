(function($){
    $.fn.linenumbers = function(inOpts){
        // Settings and Defaults
        var opt = $.extend({
            col_width: '65px',
            start: 1,
            digits: 3.
        },inOpts);
        // Remove existing div and the textarea from previous run
        $("[data-name='linenumbers']").remove();
        // Function run
        return this.each(function(){
            // Get some numbers sorted out for the CSS changes
            var textareaWidth = $(this).prop("offsetWidth");
            var textareaHeight = $(this).prop("offsetHeight");
            var newTextareaWidth = parseInt(textareaWidth)-parseInt(opt.col_width);
            // Create the new textarea and style it
            $(this).before('<textarea data-name="linenumbers" style="width:'+newTextareaWidth+'px;height:'+textareaHeight+'px;float:left;margin-right:'+'-'+newTextareaWidth+'px;white-space:pre;overflow:hidden;" disabled="disabled"></textarea>');
            // Edit the existing textarea's styles
            $(this).css({'width':newTextareaWidth+'px','height':textareaHeight+'px','float':'right'});
            // Add a clearing div.
            $(this).after('<div style="clear:both;"></div>');
            // Define a simple variable for the line-numbers box
            var lnbox = $(this).parent().find('textarea[disabled="disabled"]');
            // Bind some actions to all sorts of events that may change it's contents
            $(this).bind('blur focus change keyup keydown',function(){
                // Break apart and regex the lines, everything to spaces sans linebreaks
                var lines = "\n"+$(this).val();
                lines = lines.match(/[^\n]*\n[^\n]*/gi);
                // declare output var
                var lineNumberOutput='';
                // declare spacers and max_spacers vars, and set defaults
                var maxSpacers = ''; var spacers = '';
                for(var i = 0;i<opt.digits;i++){
                    maxSpacers += ' ';
                }
                // Loop through and process each line
                $.each(lines,function(k){
                    // Add a line if not blank
                    if(k !== 0){
                        lineNumberOutput += "\n";
                    }
                    // Determine the appropriate number of leading spaces
                    var lencheck = k+opt.start+'!';
                    spacers = maxSpacers.substr(lencheck.length-1);
                    // Add the line with out line number, to the output variable
                    lineNumberOutput += spacers+(k+opt.start)+':';
                });
                // Give the text area out modified content.
                $(lnbox).val(lineNumberOutput);
                // Change scroll position as they type, makes sure they stay in sync
                $(lnbox).scrollTop($(this).scrollTop());
            });
            // Lock scrolling together, for mouse-wheel scrolling
            $(this).scroll(function(){
                $(lnbox).scrollTop($(this).scrollTop());
            });
            // Fire it off once to get things started
            $(this).trigger('keyup');
        });
    };
})(jQuery);
$('textarea').linenumbers();