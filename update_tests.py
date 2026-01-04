import os
import re

# Pfad zum Test-Verzeichnis
test_dir = r"CustomWPFControls.Tests"

# Patterns zum Ersetzen
patterns = [
    # Pattern 1: new CollectionViewModel<TestDto, TestViewModel>(_fixture.DataStores, _fixture.ViewModelFactory, _fixture.ComparerService)
    (
        r'new CollectionViewModel<TestDto, TestViewModel>\(\s*_fixture\.DataStores,\s*_fixture\.ViewModelFactory,\s*_fixture\.ComparerService\)',
        'new CollectionViewModel<TestDto, TestViewModel>(_fixture.Services, _fixture.ViewModelFactory)'
    ),
    # Pattern 2: new EditableCollectionViewModel
    (
        r'new EditableCollectionViewModel<TestDto, TestViewModel>\(\s*_fixture\.DataStores,\s*_fixture\.ViewModelFactory,\s*_fixture\.ComparerService\)',
        'new EditableCollectionViewModel<TestDto, TestViewModel>(_fixture.Services, _fixture.ViewModelFactory)'
    ),
    # Pattern 3: new ViewModels.CollectionViewModel
    (
        r'new ViewModels\.CollectionViewModel<TestDto, TestViewModel>\(\s*_fixture\.DataStores,\s*_fixture\.ViewModelFactory,\s*_fixture\.ComparerService\)',
        'new ViewModels.CollectionViewModel<TestDto, TestViewModel>(_fixture.Services, _fixture.ViewModelFactory)'
    ),
]

def update_file(filepath):
    """Aktualisiert eine einzelne Datei"""
    try:
        with open(filepath, 'r', encoding='utf-8') as f:
            content = f.read()
        
        original_content = content
        
        # Alle Patterns anwenden
        for pattern, replacement in patterns:
            content = re.sub(pattern, replacement, content)
        
        # Nur schreiben, wenn Änderungen vorgenommen wurden
        if content != original_content:
            with open(filepath, 'w', encoding='utf-8') as f:
                f.write(content)
            return True
        return False
    except Exception as e:
        print(f"Error processing {filepath}: {e}")
        return False

def main():
    """Hauptfunktion"""
    updated_count = 0
    total_count = 0
    
    # Alle .cs-Dateien im Test-Verzeichnis durchsuchen
    for root, dirs, files in os.walk(test_dir):
        for file in files:
            if file.endswith('.cs'):
                filepath = os.path.join(root, file)
                total_count += 1
                
                # Datei aktualisieren
                if update_file(filepath):
                    updated_count += 1
                    print(f"Updated: {filepath}")
    
    print(f"\nTotal files checked: {total_count}")
    print(f"Files updated: {updated_count}")

if __name__ == "__main__":
    main()
