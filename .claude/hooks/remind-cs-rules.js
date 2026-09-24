const fs = require("fs");

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

  // Path-scoped rules load when Claude reads a matching file, so an edit of an
  // existing .cs already has them in context. Only creating a new .cs can
  // happen without that read: a Write to a path that does not exist yet, or
  // the Unity script-creation tools (which may pass a directory in `path`).
  const isNewFileWrite =
    toolName === "Write" && /scripts[\\/].*\.cs$/i.test(path) && !fs.existsSync(path);
  const isUnityCreate =
    (toolName === "mcp__unityMCP__create_script" ||
      (toolName === "mcp__unityMCP__manage_script" && toolInput.action === "create")) &&
    /scripts([\\/]|$)/i.test(path);

  if (!isNewFileWrite && !isUnityCreate) {
    return;
  }

  process.stdout.write(JSON.stringify({
    hookSpecificOutput: {
      hookEventName: "PreToolUse",
      additionalContext:
        "Creating a new .cs: path-scoped rules load only on Read. If .claude/rules (architecture, design, codestyle, exceptions, rider-mcp) are not in context yet, read them and check the new file against them. New feature / Model / Service / View / Presenter / UI screen / Performer / Loader / Provider / InputHandler → also read .claude/reference/feature-anatomy.md",
    },
  }));
});
