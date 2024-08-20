using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace testAPP
{
    public partial class library＿management : Form
    {
        private List<string[]> data = new List<string[]>(); // 데이터를 저장할 리스트
        private List<string> changes = new List<string>(); // 변경 내역을 저장할 리스트
        private SortOrder sortOrder = SortOrder.Ascending; // 정렬 순서
        private int sortColumn = -1; // 정렬할 컬럼의 인덱스
        private int selectedIndex = -1; // 선택된 항목의 인덱스를 저장할 변수
        private bool isUpdatingFromListView = false; // 리스트뷰에서 텍스트박스를 업데이트 중인지 여부

        private string connectionString = "Host=192.168.201.151;Username=postgres;Password=12345678;Database=internTest"; // PostgreSQL 연결 문자열

        // 변경된 데이터를 추적하기 위한 리스트
        private List<string[]> addedData = new List<string[]>();
        private List<int> deletedIds = new List<int>();


        public library＿management()
        {
            InitializeComponent();

            // ListView의 열을 동적으로 설정
            SetupListViewColumns();

            // 콤보박스 초기화
            SetupComboBox();

            // 데이터베이스에서 데이터 로드
            LoadDataFromDatabase();

            // 전체 도서 수 표시
            total_books.Text = "전체 도서수 : " + data.Count.ToString();

            // ColumnClick 이벤트 핸들러 추가
            lv_list.ColumnClick += new ColumnClickEventHandler(list_ColumnClick);

            // SelectedIndexChanged 이벤트 핸들러 추가
            lv_list.SelectedIndexChanged += new EventHandler(lv_list_SelectedIndexChanged);

            // 결과 보고 라벨 업데이트
            results_report2.Text = "데이터를 불러왔습니다.";

            // 텍스트박스 TextChanged 이벤트 핸들러 추가
            tb_title.TextChanged += new EventHandler(tb_TextChanged);
            tb_writer.TextChanged += new EventHandler(tb_TextChanged);
            tb_genre.TextChanged += new EventHandler(tb_TextChanged);
            tb_description.TextChanged += new EventHandler(tb_TextChanged);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        /*// 테이블의 칼럼을 ListView에 동적으로 추가
        private void SetupListViewColumns()
        {
            lv_list.Columns.Clear();

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT column_name, data_type FROM information_schema.columns WHERE table_name = 'book'", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string columnName = reader["column_name"].ToString();
                            string dataType = reader["data_type"].ToString();

                            // 열 너비를 데이터 유형에 따라 결정 (여기서는 임의로 설정)
                            int columnWidth = dataType == "text" ? 300 : 100;

                            // 열 추가
                            lv_list.Columns.Add(columnName, columnWidth);
                        }
                    }
                }
            }

            lv_list.View = View.Details; // ListView를 상세보기로 설정
        }*/

        // 테이블의 칼럼을 ListView에 고정된 방식으로 추가
        private void SetupListViewColumns()
        {
            lv_list.Columns.Clear();

            // 고정된 컬럼 이름과 너비 설정
            //lv_list.Columns.Add("", 0); // ID 열 오른쪽 정렬
            lv_list.Columns.Add("ID", 120, HorizontalAlignment.Right); // ID 열 오른쪽 정렬
            lv_list.Columns.Add("Title", 170, HorizontalAlignment.Center); // Title 열 가운데 정렬
            lv_list.Columns.Add("Writer", 100);
            lv_list.Columns.Add("Genre", 70);
            lv_list.Columns.Add("Description", 300);

            lv_list.View = View.Details; // ListView를 상세보기로 설정
        }




         
        // 데이터베이스에서 데이터 로드
        private void LoadDataFromDatabase()
        {
            int maxIdLength = 0; // ID 열의 최대 자릿수를 저장할 변수

            // 첫 번째 패스: ID 열의 최대 자릿수 계산
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT id FROM book", conn))//id값 가져오기
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idLength = reader["id"].ToString().Length;      //id값 길이 계산
                            if (idLength > maxIdLength)
                            {
                                maxIdLength = idLength;     //id열의 최대 자릿수 저장
                            }
                        }
                    }
                }
            }

            // 두 번째 패스: 데이터 로드 및 포맷팅
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM book", conn))     //테이블의 모든 데이터 읽어오기
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        data.Clear(); // 기존 데이터를 지움
                        while (reader.Read())
                        {
                            string[] row = new string[reader.FieldCount];
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (i == 0) // ID 열만 포맷팅
                                {
                                    row[i] = reader[i].ToString().PadLeft(maxIdLength, '0');
                                }
                                else
                                {
                                    row[i] = reader[i].ToString();
                                }
                            }
                            data.Add(row); // 새 데이터를 추가
                        }
                    }
                }
            }
            UpdateListView(); // ListView를 업데이트

        }


        // 검색 버튼 클릭 시
        private void search_Click(object sender, EventArgs e)
        {
            string searchText = tb_search.Text.ToLower(); // 검색어를 소문자로 변환
            string selectedColumn = cb_filter.SelectedItem.ToString(); // 선택된 콤보박스 항목
            UpdateListView(selectedColumn, searchText); // ListView를 업데이트
        }

        // 임시 ID 생성 함수
        // 임시 ID 생성 함수
        private string GenerateTempId()
        {
            return "*"; // 단순히 별표 하나를 반환
        }


        // 리스트 칼럼 클릭 시 정렬
        private void list_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == sortColumn)
            {
                // 기존칼럼을 눌렀을때 정렬 방향을 변경
                sortOrder = (sortOrder == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                // 새로운 칼럼을 오름차순 정렬
                sortColumn = e.Column;
                sortOrder = SortOrder.Ascending;
            }

            // list뷰 정렬
            lv_list.ListViewItemSorter = new ListViewItemComparer(e.Column, sortOrder);
        }

        // ListViewItemComparer 클래스: ListView 정렬을 위한 클래스
        public class ListViewItemComparer : IComparer
        {
            private int col; // 정렬할 컬럼
            private SortOrder order; // 정렬 순서

            public ListViewItemComparer()
            {
                col = 0;
                order = SortOrder.Ascending;
            }

            public ListViewItemComparer(int column, SortOrder order)
            {
                col = column;
                this.order = order;
            }

            public int Compare(object x, object y)
            {
                string text1 = ((ListViewItem)x).SubItems[col].Text;
                string text2 = ((ListViewItem)y).SubItems[col].Text;

                // 문자열 앞부분의 문자 부분 비교
                int stringCompareResult = String.Compare(
                    new string(text1.TakeWhile(char.IsLetter).ToArray()),
                    new string(text2.TakeWhile(char.IsLetter).ToArray())
                );

                if (stringCompareResult == 0) // 문자가 동일한 경우
                {
                    // 숫자 부분을 추출하여 비교
                    string numPart1 = new string(text1.SkipWhile(char.IsLetter).ToArray());
                    string numPart2 = new string(text2.SkipWhile(char.IsLetter).ToArray());

                    if (int.TryParse(numPart1, out int num1) && int.TryParse(numPart2, out int num2))
                    {
                        stringCompareResult = num1.CompareTo(num2);
                    }
                    else
                    {
                        stringCompareResult = String.Compare(numPart1, numPart2);
                    }
                }

                // 정렬 순서 적용
                if (order == SortOrder.Descending)
                    stringCompareResult *= -1;

                return stringCompareResult;
            }
        }

        /*
        // 삽입 버튼 클릭 시
        private async void insert_Click(object sender, EventArgs e)
        {
            string title = tb_title.Text.Trim();
            string writer = tb_writer.Text.Trim();
            string genre = tb_genre.Text.Trim();
            string description = tb_description.Text.Trim();

            if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(writer) &&
                string.IsNullOrEmpty(genre) && string.IsNullOrEmpty(description))
            {
                MessageBox.Show("모든 항목이 빈칸일 수 없습니다.");
                return;
            }

            string tempId = GenerateTempId();

            // Create SQL insert query using parameters
            string query = "INSERT INTO book (title, writer, genre, description) VALUES (@Title, @Writer, @Genre, @Description)";
            using (var conn = new NpgsqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("Title", title);
                    cmd.Parameters.AddWithValue("Writer", writer);
                    cmd.Parameters.AddWithValue("Genre", genre);
                    cmd.Parameters.AddWithValue("Description", description);

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            //ListView 업데이트
            string[] row = new string[] { tempId, title, writer, genre, description };
            data.Add(row);
            UpdateListView();

            //텍스트박스 초기화
            total_books.Text = "전체 도서수 : " + data.Count.ToString();
            tb_title.Text = "";
            tb_writer.Text = "";
            tb_genre.Text = "";
            tb_description.Text = "";

            results_report2.Text = "데이터가 삽입되었습니다.";
            ResetLabels();
        }*/

        // 삽입 버튼 클릭 시
        // 삽입 버튼 클릭 시
        private void insert_Click(object sender, EventArgs e)
        {
            string title = tb_title.Text.Trim();
            string writer = tb_writer.Text.Trim();
            string genre = tb_genre.Text.Trim();
            string description = tb_description.Text.Trim();

            if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(writer) &&
                string.IsNullOrEmpty(genre) && string.IsNullOrEmpty(description))
            {
                MessageBox.Show("모든 항목이 빈칸일 수 없습니다.");
                return;
            }

            // 임시 ID 생성
            string tempId = GenerateTempId();

            // Create new row with formatted ID
            string[] newRow = new string[] { tempId, title, writer, genre, description };
            addedData.Add(newRow);

            ListViewItem item = new ListViewItem(newRow);
            lv_list.Items.Add(item);

            // 텍스트박스 초기화
            total_books.Text = "전체 도서수 : " + lv_list.Items.Count.ToString(); // 데이터 리스트의 수를 가져옵니다.
            tb_title.Text = "";
            tb_writer.Text = "";
            tb_genre.Text = "";
            tb_description.Text = "";
        }





        /*// 수정 버튼 클릭 시
        private void bt_edit_Click(object sender, EventArgs e)
        {
            if (selectedIndex != -1)
            {
                string id = lv_list.SelectedItems[0].SubItems[0].Text; // 선택된 항목의 ID

                // 입력된 값을 읽음
                string newTitle = tb_title.Text.Trim();
                string newWriter = tb_writer.Text.Trim();
                string newGenre = tb_genre.Text.Trim();
                string newDescription = tb_description.Text.Trim();

                // 기존 값을 읽음
                string oldTitle = lv_list.SelectedItems[0].SubItems[1].Text;
                string oldWriter = lv_list.SelectedItems[0].SubItems[2].Text;
                string oldGenre = lv_list.SelectedItems[0].SubItems[3].Text;
                string oldDescription = lv_list.SelectedItems[0].SubItems[4].Text;

                List<string> updates = new List<string>();

                if (newTitle != oldTitle) updates.Add($"title='{newTitle}'");
                if (newWriter != oldWriter) updates.Add($"writer='{newWriter}'");
                if (newGenre != oldGenre) updates.Add($"genre='{newGenre}'");
                if (newDescription != oldDescription) updates.Add($"description='{newDescription}'");

                if (updates.Count > 0)
                {
                    if (id.Contains("*"))
                    {
                        // 임시 데이터인 경우 기존 임시 데이터를 삭제하고 새 임시 데이터를 추가
                        data.RemoveAt(selectedIndex);
                        lv_list.Items.RemoveAt(selectedIndex);

                        string[] newRow = new string[] { GenerateTempId(), newTitle, newWriter, newGenre, newDescription };
                        data.Add(newRow);
                        UpdateListView();
                    }
                    else
                    {
                        // 데이터베이스에 있는 데이터인 경우 수정 쿼리 생성
                        string updateQuery = $"UPDATE book SET {string.Join(", ", updates)} WHERE id={id}";
                        changes.Add(updateQuery);

                        // ListView 업데이트
                        lv_list.SelectedItems[0].SubItems[1].Text = newTitle;
                        lv_list.SelectedItems[0].SubItems[2].Text = newWriter;
                        lv_list.SelectedItems[0].SubItems[3].Text = newGenre;
                        lv_list.SelectedItems[0].SubItems[4].Text = newDescription;
                    }

                    // 텍스트박스 초기화
                    selectedIndex = -1;
                    tb_title.Text = "";
                    tb_writer.Text = "";
                    tb_genre.Text = "";
                    tb_description.Text = "";

                    // 결과 보고 라벨 업데이트
                    results_report2.Text = "데이터가 수정되었습니다.";
                }
                else
                {
                    MessageBox.Show("변경된 내용이 없습니다.");
                }
            }
            else
            {
                MessageBox.Show("수정할 항목을 선택하세요.");
            }

            // 별표 제거
            ResetLabels();
        }*/

        // 수정 버튼 클릭 시
        private void bt_edit_Click(object sender, EventArgs e)
        {
            if (selectedIndex != -1)
            {
                string newTitle = tb_title.Text.Trim();
                string newWriter = tb_writer.Text.Trim();
                string newGenre = tb_genre.Text.Trim();
                string newDescription = tb_description.Text.Trim();

                var selectedItem = lv_list.SelectedItems[0];
                selectedItem.SubItems[1].Text = newTitle;
                selectedItem.SubItems[2].Text = newWriter;
                selectedItem.SubItems[3].Text = newGenre;
                selectedItem.SubItems[4].Text = newDescription;

                // 결과 보고 라벨 업데이트
                results_report2.Text = "데이터가 수정되었습니다.";

                // 수정된 항목을 modifiedData에 추가
                string[] modifiedRow = new string[] { selectedItem.SubItems[0].Text.PadLeft(6, '0'), newTitle, newWriter, newGenre, newDescription };
                modifiedData.Add(modifiedRow);

                originalTitle = newTitle;
                originalWriter = newWriter;
                originalGenre = newGenre;
                originalDescription = newDescription;
            }
            else
            {
                MessageBox.Show("수정할 항목을 선택하세요.");
            }
            // 별표 제거
            ResetLabels();
        }

        /*
        // 삭제 버튼 클릭 시
        private void bt_delete_Click(object sender, EventArgs e)
        {
            if (selectedIndex != -1)
            {
                string id = lv_list.SelectedItems[0].SubItems[0].Text; // 선택된 항목의 ID

                if (!id.Contains("*"))
                {
                    // 데이터베이스에 있는 데이터인 경우에만 삭제 쿼리를 추가
                    changes.Add($"DELETE FROM book WHERE id={id}");
                }

                // 데이터 삭제
                lv_list.Items.RemoveAt(selectedIndex);
                data.RemoveAt(selectedIndex);

                total_books.Text = "전체 도서수 : " + data.Count.ToString();

                selectedIndex = -1;
                tb_title.Text = "";
                tb_writer.Text = "";
                tb_genre.Text = "";
                tb_description.Text = "";
            }
            else
            {
                MessageBox.Show("삭제할 항목을 선택하세요.");
            }

            // 결과 보고 라벨 업데이트
            results_report2.Text = "데이터가 삭제되었습니다.";

            // 별표 제거
            ResetLabels();
        }*/

        // 삭제 버튼 클릭 시
        // 삭제 버튼 클릭 시
        private void bt_delete_Click(object sender, EventArgs e)
        {
            // 선택된 항목이 있는지 확인
            if (lv_list.SelectedItems.Count > 0)
            {
                // 선택된 모든 항목을 삭제
                foreach (ListViewItem selectedItem in lv_list.SelectedItems)
                {
                    string id = selectedItem.SubItems[0].Text; // 선택된 항목의 ID

                    if (id.Contains("*"))
                    {
                        // 임시 데이터인 경우
                        int indexToRemove = selectedItem.Index;

                        // 데이터 리스트에서 임시 데이터 삭제
                        if (indexToRemove >= 0 && indexToRemove < data.Count)
                        {
                            data.RemoveAt(indexToRemove);
                        }

                        // ListView에서 항목 삭제
                        if (indexToRemove >= 0 && indexToRemove < lv_list.Items.Count)
                        {
                            lv_list.Items.RemoveAt(indexToRemove);
                        }
                    }
                    else
                    {
                        // 데이터베이스에 저장된 데이터인 경우
                        int idValue;
                        if (int.TryParse(id, out idValue))
                        {
                            // 삭제할 항목의 ID를 deletedIds 리스트에 추가
                            deletedIds.Add(idValue);

                            // ListView에서 항목 삭제
                            int indexToRemove = selectedItem.Index;
                            if (indexToRemove >= 0 && indexToRemove < lv_list.Items.Count)
                            {
                                lv_list.Items.RemoveAt(indexToRemove); // ListView에서 삭제
                                data.RemoveAt(indexToRemove); // 데이터 리스트에서 삭제
                            }
                        }
                        else
                        {
                            MessageBox.Show("유효하지 않은 ID입니다.");
                        }
                    }
                }

                // 전체 도서 수 업데이트
                total_books.Text = "전체 도서수 : " + data.Count.ToString();

                // 텍스트 박스 및 선택된 항목 초기화
                tb_clear();

                // 결과 보고 라벨 업데이트
                results_report2.Text = "선택된 데이터가 삭제되었습니다.";
                ResetLabels();
            }
            else
            {
                MessageBox.Show("삭제할 항목을 선택하세요.");
            }
        }




        // 콤보박스 칼럼 검색
        private void SetupComboBox()
        {
            // 콤보박스 초기화
            cb_filter.Items.Clear();
            cb_filter.Items.Add("전체");

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT column_name FROM information_schema.columns WHERE table_name = 'book'", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string columnName = reader["column_name"].ToString();
                            if (columnName != "id") // 'id' 열은 필터에 포함하지 않음
                            {
                                cb_filter.Items.Add(columnName);
                            }
                        }
                    }
                }
            }

            cb_filter.SelectedIndex = 0; // 기본 설정은 '전체'
        }

        // ListView 업데이트
        private void UpdateListView(string filterColumn = "", string searchText = "")
        {
            lv_list.Items.Clear();

            foreach (string[] row in data)
            {
                bool addRow = true;

                if (!string.IsNullOrEmpty(searchText))
                {
                    if (!string.IsNullOrEmpty(filterColumn) && filterColumn != "전체")
                    {
                        int columnIndex = lv_list.Columns.Cast<ColumnHeader>().ToList().FindIndex(c => c.Text == filterColumn);
                        if (columnIndex != -1)
                        {
                            addRow = detailed_Search(row[columnIndex], searchText);
                        }
                    }
                    else
                    {
                        addRow = row.Any(col => detailed_Search(col, searchText));
                    }
                }

                if (addRow)
                {
                    ListViewItem item = new ListViewItem(row);
                    lv_list.Items.Add(item);
                }
            }
        }


        /*
        // 리스트뷰 항목 선택 이벤트 핸들러 추가
        private void lv_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lv_list.SelectedItems.Count > 0)
            {
                isUpdatingFromListView = true; // 리스트뷰에서 업데이트 시작

                selectedIndex = lv_list.SelectedIndices[0]; // 선택된 항목의 인덱스 저장

                // 선택된 항목의 텍스트박스를 업데이트
                tb_title.Text = lv_list.SelectedItems[0].SubItems[1].Text;
                tb_writer.Text = lv_list.SelectedItems[0].SubItems[2].Text;
                tb_genre.Text = lv_list.SelectedItems[0].SubItems[3].Text;
                tb_description.Text = lv_list.SelectedItems[0].SubItems[4].Text;

                isUpdatingFromListView = false; // 리스트뷰에서 업데이트 종료
            }
        }*/

        // 리스트뷰 선택 항목 선택시 텍스트박스에 출력
        private void lv_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lv_list.SelectedItems.Count > 0)
            {
                isUpdatingFromListView = true; // 리스트뷰에서 업데이트 시작

                selectedIndex = lv_list.SelectedIndices[0]; // 선택된 항목의 인덱스 저장

                // 선택된 항목의 텍스트박스를 업데이트
                tb_title.Text = lv_list.SelectedItems[0].SubItems[1].Text;
                tb_writer.Text = lv_list.SelectedItems[0].SubItems[2].Text;
                tb_genre.Text = lv_list.SelectedItems[0].SubItems[3].Text;
                tb_description.Text = lv_list.SelectedItems[0].SubItems[4].Text;

                isUpdatingFromListView = false; // 리스트뷰에서 업데이트 종료
            }
        }

        private List<string[]> modifiedData = new List<string[]>();

        /*
        // 적용 버튼 클릭 시
        private void bt_apply_Click(object sender, EventArgs e)
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                foreach (string change in changes)
                {
                    using (var cmd = new NpgsqlCommand(change, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            changes.Clear();
            LoadDataFromDatabase();

            // 결과 보고 라벨 업데이트
            results_report2.Text = "데이터베이스에 저장되었습니다.";

            // 별표 제거
            ResetLabels();
        }*/



        // 저장 버튼 클릭 시
        private void bt_apply_Click(object sender, EventArgs e)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    // 추가된 항목을 데이터베이스에 삽입
                    foreach (var Row in addedData)
                    {
                        using (var cmd = new NpgsqlCommand("INSERT INTO book (title, writer, genre, description) VALUES (@title, @writer, @genre, @description)", conn))
                        {
                            cmd.Parameters.AddWithValue("title", Row[1]);
                            cmd.Parameters.AddWithValue("writer", Row[2]);
                            cmd.Parameters.AddWithValue("genre", Row[3]);
                            cmd.Parameters.AddWithValue("description", Row[4]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    addedData.Clear();

                    // 수정된 항목을 데이터베이스에 업데이트
                    foreach (var Row in modifiedData)
                    {
                        using (var cmd = new NpgsqlCommand("UPDATE book SET title = @title, writer = @writer, genre = @genre, description = @description WHERE id = @id", conn))
                        {
                            // 여기서 Row[0]을 long 타입으로 변환
                            cmd.Parameters.AddWithValue("id", Convert.ToInt64(Row[0])); // or long.Parse(Row[0].ToString())
                            cmd.Parameters.AddWithValue("title", Row[1]);
                            cmd.Parameters.AddWithValue("writer", Row[2]);
                            cmd.Parameters.AddWithValue("genre", Row[3]);
                            cmd.Parameters.AddWithValue("description", Row[4]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    modifiedData.Clear();

                    // 삭제된 항목을 데이터베이스에서 제거
                    foreach (var id in deletedIds)
                    {
                        using (var cmd = new NpgsqlCommand("DELETE FROM book WHERE id = @id", conn))
                        {
                            // 여기서 id를 long 타입으로 변환
                            cmd.Parameters.AddWithValue("id", Convert.ToInt64(id)); // or long.Parse(id.ToString())
                            cmd.ExecuteNonQuery();
                        }
                    }
                    deletedIds.Clear();
                }

                // 데이터베이스에서 데이터 다시 로드
                LoadDataFromDatabase();

                // 변경 사항 초기화
                addedData.Clear();
                deletedIds.Clear();
                modifiedData.Clear();

                // 전체 도서수 업데이트
                total_books.Text = "전체 도서수 : " + data.Count.ToString();

                // 결과 보고 라벨 업데이트
                results_report2.Text = "변경사항이 데이터베이스에 저장되었습니다.";

                // 별표 제거
                ResetLabels();
            }
            catch (Exception ex)
            {
                // 예외 발생 시 오류 메시지 출력
                MessageBox.Show($"오류가 발생했습니다: {ex.Message}");
            }
        }




        // 새로고침 버튼 클릭 시
        private void bt_refresh_Click(object sender, EventArgs e)
        {
            // 데이터베이스에서 최신 데이터 로드
            LoadDataFromDatabase();

            // 전체 도서 수 업데이트
            total_books.Text = "전체 도서수 : " + data.Count.ToString();

            // 텍스트 박스 및 선택된 항목 초기화
            tb_clear();

            // 선택된 항목 초기화
            selectedIndex = -1;

            // ListView 업데이트
            UpdateListView();

            // 결과 보고 라벨 업데이트
            results_report2.Text = "데이터를 새로 고침 했습니다.";

            // 별표 제거
            ResetLabels();
        }

        //텍스트박스 청소
        public void tb_clear()
        {
            tb_title.Text = "";
            tb_writer.Text = "";
            tb_genre.Text = "";
            tb_description.Text = "";
        }

        // 수정된 라벨 별표 추가
        private void tb_TextChanged(object sender, EventArgs e)
        {
            if (!isUpdatingFromListView) // 리스트뷰에서 업데이트 중이 아닐 때만 별표 추가
            {
                if (sender == tb_title)
                    title_label.Text = "Title*";
                else if (sender == tb_writer)
                    writer_label.Text = "Writer*";
                else if (sender == tb_genre)
                    genre_label.Text = "Genre*";
                else if (sender == tb_description)
                    description_label.Text = "Description*";
            }
        }

        // 라벨 별표 제거 함수
        private void ResetLabels()
        {
            title_label.Text = "Title";
            writer_label.Text = "Writer";
            genre_label.Text = "Genre";
            description_label.Text = "Description";
        }
        private string originalTitle = "";
        private string originalWriter = "";
        private string originalGenre = "";
        private string originalDescription = "";

        //검색 엔터키 이벤트
        private void tb_search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter){
                search_Click(sender, e);
            }
        }

        //검색조건 추가
        private bool detailed_Search(string text, string pattern)
        {
            // *로 시작하고 *로 끝나는 경우 (중간에 해당하는 문자열이 포함되는지 체크)
            if (pattern.StartsWith("*") && pattern.EndsWith("*"))
            {
                string subPattern = pattern.Trim('*').ToLower();        //ToLower()대소문자 무시 Trim()공백제거
                return text.ToLower().Contains(subPattern);
            }
            // *로 시작하는 경우 (문자열 끝이 패턴으로 끝나는지 체크)
            else if (pattern.StartsWith("*"))
            {
                string subPattern = pattern.TrimStart('*').ToLower();
                return text.ToLower().EndsWith(subPattern);
            }
            // *로 끝나는 경우 (문자열 시작이 패턴으로 시작하는지 체크)
            else if (pattern.EndsWith("*"))
            {
                string subPattern = pattern.TrimEnd('*').ToLower();
                return text.ToLower().StartsWith(subPattern);
            }
            // *가 없는 경우 (완전 일치 체크)
            else
            {
                return text.ToLower() == pattern.ToLower();
            }
        }
    }
}