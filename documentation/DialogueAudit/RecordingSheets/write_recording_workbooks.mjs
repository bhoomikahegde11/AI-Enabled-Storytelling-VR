import fs from "node:fs/promises";
import path from "node:path";
import { createRequire } from "node:module";

const [, , payloadPath, outputDir, nodeModules] = process.argv;
if (!payloadPath || !outputDir || !nodeModules) throw new Error("Expected payload JSON, output directory, and node_modules path");
const runtimeRequire = createRequire(path.join(nodeModules, "recording_sheets_loader.cjs"));
const { SpreadsheetFile, Workbook } = runtimeRequire("@oai/artifact-tool");
const { books } = JSON.parse(await fs.readFile(payloadPath, "utf8"));

const widths = {
  "Dialogue Overview": [19, 23, 25, 13, 12, 78, 34, 15, 95, 16, 64],
  "Static Recording Segments": [20, 19, 15, 78, 27, 60, 25, 20, 55],
  "Number Library": [13, 26, 26, 20, 55],
  "Dynamic Vocabulary": [28, 18, 27, 36, 24, 20, 66],
  "Assembly Manifest": [19, 15, 25, 68, 92, 100, 14, 70],
  "Recording Summary": [38, 24, 82],
};
for (const [bookIndex, book] of books.entries()) {
  const workbook = Workbook.create();
  for (const [name, data] of Object.entries(book.sheets)) {
    const sheet = workbook.worksheets.add(name);
    sheet.showGridLines = false;
    const width = data.headers.length;
    sheet.getRangeByIndexes(0, 0, 1, width).values = [data.headers];
    for (let start = 0; start < data.rows.length; start += 250) {
      const chunk = data.rows.slice(start, start + 250);
      sheet.getRangeByIndexes(start + 1, 0, chunk.length, width).values =
        chunk.map(row => data.headers.map(header => row[header] ?? ""));
    }
    sheet.getRangeByIndexes(0, 0, data.rows.length + 1, width).format.font = {
      name: "Aptos", size: 11, color: "#302720",
    };
    const header = sheet.getRangeByIndexes(0, 0, 1, width);
    header.format = {
      fill: "#33251E",
      font: { name: "Aptos", size: 11, bold: true, color: "#F4E6CC" },
      rowHeight: 31,
      verticalAlignment: "center",
      wrapText: true,
    };
    sheet.freezePanes.freezeRows(1);
    for (let col = 0; col < width; col++) {
      sheet.getRangeByIndexes(0, col, data.rows.length + 1, 1).format.columnWidth = widths[name][col];
    }
    if (data.rows.length) {
      const body = sheet.getRangeByIndexes(1, 0, data.rows.length, width);
      body.format.rowHeight = name === "Dialogue Overview" ? 44 : name === "Assembly Manifest" ? 43 : 34;
      body.format.verticalAlignment = "center";
      body.format.wrapText = true;
      if (name === "Recording Summary") {
        sheet.getRangeByIndexes(1, 1, data.rows.length, 1).format.font.bold = true;
      }
    }
  }
  workbook.recalculate();
  const summary = await workbook.inspect({
    kind: "table", range: "Recording Summary!A1:C10", include: "values",
    tableMaxRows: 10, tableMaxCols: 3,
  });
  if (!summary.ndjson.includes("Total planned recordable clips")) {
    throw new Error(`${book.name}: summary inspection failed`);
  }
  if (bookIndex === 0) {
    for (const name of Object.keys(book.sheets)) {
      const preview = await workbook.render({ sheetName: name, range: `A1:C7`, scale: 1, format: "png" });
      if ((await preview.arrayBuffer()).byteLength === 0) throw new Error(`${name}: empty rendered preview`);
    }
  }
  const destination = path.join(outputDir, book.name);
  const temporary = `${destination}.generating`;
  const xlsx = await SpreadsheetFile.exportXlsx(workbook);
  await xlsx.save(temporary);
  await fs.rename(temporary, destination);
  await fs.rm(`${temporary}.inspect.ndjson`, { force: true });
  process.stdout.write(`Wrote ${book.name}: ${book.counts.templates} templates, ${book.counts.planned_clips} planned clips\n`);
}
