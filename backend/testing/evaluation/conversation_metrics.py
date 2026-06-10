import numpy as np
import json
import os

class MetricsAccumulator:
    def __init__(self):
        self.total_cases = 0
        self.passed_cases = 0
        self.failed_cases = 0
        
        # Category metrics
        self.categories = {} # category -> {"total": 0, "passed": 0, "failed": 0}
        
        # Latencies
        self.latencies = {
            "total": [],
            "stt": [],
            "intent": [],
            "llm": [],
            "tts": []
        }
        
        # Failures list
        self.failures = []

    def record_case(self, category, is_pass, latencies=None, failure_detail=None):
        self.total_cases += 1
        if is_pass:
            self.passed_cases += 1
        else:
            self.failed_cases += 1
            
        if category not in self.categories:
            self.categories[category] = {"total": 0, "passed": 0, "failed": 0}
            
        self.categories[category]["total"] += 1
        if is_pass:
            self.categories[category]["passed"] += 1
        else:
            self.categories[category]["failed"] += 1
            
        if latencies:
            for k, v in latencies.items():
                if k in self.latencies and v is not None:
                    self.latencies[k].append(v)
                    
        if not is_pass and failure_detail:
            self.failures.append(failure_detail)

    def calculate_percentile(self, values, percentile):
        if not values:
            return 0.0
        return float(np.percentile(values, percentile))

    def get_summary(self):
        category_summaries = {}
        for cat, data in self.categories.items():
            total = data["total"]
            passed = data["passed"]
            accuracy = (passed / total) * 100.0 if total > 0 else 0.0
            category_summaries[cat] = {
                "total": total,
                "passed": passed,
                "failed": data["failed"],
                "accuracy": round(accuracy, 2)
            }
            
        overall_accuracy = (self.passed_cases / self.total_cases) * 100.0 if self.total_cases > 0 else 0.0
        
        latency_stats = {}
        for k, vals in self.latencies.items():
            if vals:
                latency_stats[k] = {
                    "avg": round(float(np.mean(vals)), 2),
                    "p50": round(self.calculate_percentile(vals, 50), 2),
                    "p90": round(self.calculate_percentile(vals, 90), 2),
                    "p95": round(self.calculate_percentile(vals, 95), 2),
                    "max": round(float(np.max(vals)), 2),
                    "min": round(float(np.min(vals)), 2)
                }
            else:
                latency_stats[k] = {"avg": 0.0, "p50": 0.0, "p90": 0.0, "p95": 0.0, "max": 0.0, "min": 0.0}

        return {
            "total_cases": self.total_cases,
            "passed_cases": self.passed_cases,
            "failed_cases": self.failed_cases,
            "overall_accuracy": round(overall_accuracy, 2),
            "categories": category_summaries,
            "latency": latency_stats
        }

    def export_json(self, filepath):
        os.makedirs(os.path.dirname(filepath), exist_ok=True)
        summary = self.get_summary()
        with open(filepath, "w") as f:
            json.dump(summary, f, indent=2)
        print(f"[INFO] Exported metrics to {filepath}")
