"use client";

import { Department } from "@/entities/departments/types";
import { DepartmentSelectDialog } from "@/features/Department-Select/Department-Select-Dialog";
import { useState } from "react";

export function departmentSelect() {
  const [singleValue, setSingleValue] = useState<Department | null>(null);
  const [multiValue, setMultiValue] = useState<Department[]>([]);
  return (
    <div className="grid grid-cols-1 gap-8 p-8">
      <div>
        <h2>Single</h2>
        <DepartmentSelectDialog
          mode="single"
          value={singleValue}
          onChange={setSingleValue}
        />
      </div>
      <div>
        <h2>Multiply</h2>
        <DepartmentSelectDialog
          mode="multiply"
          value={multiValue}
          onChange={setMultiValue}
        />
      </div>
    </div>
  );
}
