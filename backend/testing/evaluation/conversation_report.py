import os
import json

def compile_report(metrics_summary, failures, output_dir):
    os.makedirs(output_dir, exist_ok=True)
    
    # 1. Write failures.json
    failures_path = os.path.join(output_dir, "failures.json")
    with open(failures_path, "w") as f:
        json.dump(failures, f, indent=2)
    print(f"[INFO] Exported failures log to {failures_path}")
    
    # 2. Build report.html
    html_path = os.path.join(output_dir, "report.html")
    
    # Generate HTML content
    cat_rows = ""
    for cat, data in metrics_summary["categories"].items():
        acc_class = "pass" if data["accuracy"] >= 90 else "fail"
        cat_rows += f"""
        <tr>
            <td><strong>{cat}</strong></td>
            <td>{data['total']}</td>
            <td>{data['passed']}</td>
            <td class="{acc_class}">{data['accuracy']}%</td>
        </tr>
        """
        
    latency_rows = ""
    for component, stats in metrics_summary["latency"].items():
        latency_rows += f"""
        <tr>
            <td><strong>{component.upper()}</strong></td>
            <td>{stats['avg']} ms</td>
            <td>{stats['p50']} ms</td>
            <td>{stats['p90']} ms</td>
            <td>{stats['p95']} ms</td>
            <td>{stats['max']} ms</td>
        </tr>
        """
        
    failure_cards = ""
    if not failures:
        failure_cards = "<div class='no-failures'>No test cases failed! Dynamic understanding is fully robust.</div>"
    else:
        for idx, fail in enumerate(failures[:50]): # Display up to 50 failures in the HTML report
            context_str = json.dumps(fail.get("context", {}), indent=2)
            failure_cards += f"""
            <div class="failure-card">
                <div class="failure-header">
                    <span class="fail-badge">FAILED</span>
                    <strong>{fail.get('id', 'Unknown')}</strong> [{fail.get('category', 'N/A')}] - "{fail.get('input', '')}"
                </div>
                <div class="failure-body">
                    <p><strong>Context:</strong> <pre>{context_str}</pre></p>
                    <p><strong>Expected:</strong> {fail.get('expected', '')}</p>
                    <p><strong>Actual:</strong> {fail.get('actual', '')}</p>
                    <p><strong>Detail:</strong> {fail.get('detail', '')}</p>
                </div>
            </div>
            """
        if len(failures) > 50:
            failure_cards += f"<div class='more-failures'>and {len(failures) - 50} more failures (see failures.json)</div>"
            
    html_content = f"""<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>Vijayanagara NPC Conversation Benchmark Report</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f5f7fa;
            color: #333;
            margin: 0;
            padding: 20px;
        }}
        .container {{
            max-width: 1200px;
            margin: 0 auto;
            background: white;
            padding: 30px;
            border-radius: 12px;
            box-shadow: 0 4px 6px rgba(0,0,0,0.05);
        }}
        h1, h2 {{
            color: #2c3e50;
            border-bottom: 2px solid #ecf0f1;
            padding-bottom: 10px;
        }}
        .summary-boxes {{
            display: flex;
            gap: 20px;
            margin-bottom: 30px;
        }}
        .summary-box {{
            flex: 1;
            padding: 20px;
            border-radius: 8px;
            text-align: center;
            background: #f8f9fa;
            border: 1px solid #e9ecef;
        }}
        .summary-box.pass {{
            background: #e8f8f5;
            border-color: #a3e4d7;
            color: #117864;
        }}
        .summary-box.fail {{
            background: #fdf2e9;
            border-color: #fadbd8;
            color: #922b21;
        }}
        .summary-val {{
            font-size: 32px;
            font-weight: bold;
            margin-top: 5px;
        }}
        table {{
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 30px;
        }}
        th, td {{
            padding: 12px;
            text-align: left;
            border-bottom: 1px solid #e9ecef;
        }}
        th {{
            background-color: #f8f9fa;
            font-weight: 600;
        }}
        .pass {{
            color: #27ae60;
            font-weight: bold;
        }}
        .fail {{
            color: #c0392b;
            font-weight: bold;
        }}
        .failure-card {{
            background: #fdf2f2;
            border-left: 5px solid #ec7063;
            padding: 15px;
            margin-bottom: 15px;
            border-radius: 4px;
        }}
        .failure-header {{
            font-size: 16px;
            margin-bottom: 10px;
        }}
        .fail-badge {{
            background: #ec7063;
            color: white;
            padding: 2px 6px;
            border-radius: 3px;
            font-size: 11px;
            font-weight: bold;
            margin-right: 10px;
        }}
        pre {{
            background: #eaeded;
            padding: 8px;
            border-radius: 4px;
            font-family: monospace;
            margin: 5px 0;
            font-size: 13px;
            white-space: pre-wrap;
        }}
        .no-failures {{
            background: #ebf5fb;
            color: #2e86c1;
            padding: 20px;
            border-radius: 6px;
            text-align: center;
            font-weight: bold;
        }}
        .more-failures {{
            text-align: center;
            color: #7f8c8d;
            font-style: italic;
            margin-top: 15px;
        }}
    </style>
</head>
<body>
    <div class="container">
        <h1>Vijayanagara NPC Conversation Pipeline Benchmark</h1>
        <p>This report documents the performance, accuracy, and robust dialog outcomes of the Vijayanagara marketplace AI agent.</p>
        
        <div class="summary-boxes">
            <div class="summary-box">
                <div>Total Test Cases</div>
                <div class="summary-val">{metrics_summary['total_cases']}</div>
            </div>
            <div class="summary-box pass">
                <div>Passed Cases</div>
                <div class="summary-val">{metrics_summary['passed_cases']}</div>
            </div>
            <div class="summary-box fail">
                <div>Failed Cases</div>
                <div class="summary-val">{metrics_summary['failed_cases']}</div>
            </div>
            <div class="summary-box">
                <div>Overall Accuracy</div>
                <div class="summary-val">{metrics_summary['overall_accuracy']}%</div>
            </div>
        </div>
        
        <h2>Category Accuracy</h2>
        <table>
            <thead>
                <tr>
                    <th>Category</th>
                    <th>Total Cases</th>
                    <th>Passed Cases</th>
                    <th>Accuracy</th>
                </tr>
            </thead>
            <tbody>
                {cat_rows}
            </tbody>
        </table>
        
        <h2>Latency Analysis</h2>
        <table>
            <thead>
                <tr>
                    <th>Pipeline Stage</th>
                    <th>Average Latency</th>
                    <th>p50 (Median)</th>
                    <th>p90</th>
                    <th>p95</th>
                    <th>Maximum Latency</th>
                </tr>
            </thead>
            <tbody>
                {latency_rows}
            </tbody>
        </table>
        
        <h2>Worst Failure Cases (Top 50)</h2>
        <div class="failure-list">
            {failure_cards}
        </div>
    </div>
</body>
</html>
"""
    with open(html_path, "w", encoding="utf-8") as f:
        f.write(html_content)
    print(f"[INFO] Exported report UI to {html_path}")
