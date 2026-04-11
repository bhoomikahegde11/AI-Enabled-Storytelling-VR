from dataclasses import dataclass
from typing import Optional


@dataclass
class PlayerAction:
    intent: str
    price: Optional[int]
    quantity: Optional[int]


@dataclass
class EngineDecision:
    action: str
    price: Optional[int]
    quantity: Optional[int]
    done: bool
    reason: Optional[str] = None
    stage: Optional[str] = None
