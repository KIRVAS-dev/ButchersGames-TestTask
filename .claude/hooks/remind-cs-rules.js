let raw = "";
process.stdin.on("data", (chunk) => { raw += chunk; });
process.stdin.on("end", () => {
  let input;
  try {
    input = JSON.parse(raw);
  } catch {
    return;
  }

  const toolName = input.tool_name || "";
  const toolInput = input.tool_input || {};
  const path = toolInput.file_path || toolInput.path || toolInput.uri || "";

  // Edit/Write touch arbitrary files, so require an actual .cs path under a
  // Scripts folder. The Unity script tools (create_script, apply_text_edits,
  // script_apply_edits, manage_script) are C#-only by definition and some of
  // them pass a directory in `path` with no .cs extension (name is separate),
  // so for those a Scripts-folder path is enough.
  const isGenericEdit = toolName === "Edit" || toolName === "Write";
  const matchesPath = isGenericEdit
    ? /scripts[\\/].*\.cs$/i.test(path)
    : /scripts([\\/]|$)/i.test(path);

  if (!matchesPath) {
    return;
  }

  process.stdout.write(JSON.stringify({
    hookSpecificOutput: {
      hookEventName: "PreToolUse",
      additionalContext:
        "Перед правкой .cs в ButchersGames/Assets/_Project/Scripts проверь .claude/rules/codestyle.md, exceptions.md, architecture.md, class-design.md, method-design.md, variable-design.md, rider-mcp.md",
    },
  }));
});
