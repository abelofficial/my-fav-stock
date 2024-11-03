import React, { useState } from "react";
import axios from "axios";
import "./App.css";

function App() {
    const indices = [
        { name: "S&P 500 ETF", id: "sp500" },
        { name: "Dow Jones ETF", id: "dowjones" },
    ];
    const [selectedIndex, setSelectedIndex] = useState(null);
    const [companies, setCompanies] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [companiesPerPage] = useState(20);
    const [selectedCompany, setSelectedCompany] = useState(null);
    const [stockData, setStockData] = useState(null);
    const [sortConfig, setSortConfig] = useState({
        key: null,
        direction: "asc",
    });

    const handleIndexSelect = (symbol) => {
        setSelectedIndex(symbol);
        setCurrentPage(1);
        axios
            .get(
                `https://wlzx6sn1y2.execute-api.eu-north-1.amazonaws.com/workstation/scrapIndexStocks/${symbol}`
            )
            .then((response) => {
                setCompanies(response.data.data.result[0].results);
            })
            .catch((error) => {
                console.error("Error fetching companies:", error);
            });
    };

    const fetchStockData = (symbol) => {
        axios
            .get(
                `https://wlzx6sn1y2.execute-api.eu-north-1.amazonaws.com/workstation/scrapStockValues/${symbol}`
            )
            .then((response) => {
                setStockData(response.data.data?.result[0]);
                setSelectedCompany(symbol);
            })
            .catch((error) => {
                console.error("Error fetching stock data:", error);
            });
    };

    const handleSort = (key) => {
        let direction = "asc";
        if (sortConfig.key === key && sortConfig.direction === "asc") {
            direction = "desc";
        }
        setSortConfig({ key, direction });

        const sortedCompanies = [...companies].sort((a, b) => {
            if (a[key] < b[key]) {
                return direction === "asc" ? -1 : 1;
            }
            if (a[key] > b[key]) {
                return direction === "asc" ? 1 : -1;
            }
            return 0;
        });
        setCompanies(sortedCompanies);
    };

    const indexOfLastCompany = currentPage * companiesPerPage;
    const indexOfFirstCompany = indexOfLastCompany - companiesPerPage;
    const currentCompanies = companies.slice(
        indexOfFirstCompany,
        indexOfLastCompany
    );
    const totalPages = Math.ceil(companies.length / companiesPerPage);

    const renderPaginationButtons = () => {
        const pages = [];
        if (totalPages <= 1) return null;

        pages.push(
            <button
                key={1}
                onClick={() => setCurrentPage(1)}
                className={currentPage === 1 ? "active" : ""}
            >
                1
            </button>
        );

        if (currentPage > 3) {
            pages.push(<span key="start-ellipsis">...</span>);
        }

        for (
            let i = Math.max(2, currentPage - 1);
            i <= Math.min(totalPages - 1, currentPage + 1);
            i++
        ) {
            pages.push(
                <button
                    key={i}
                    onClick={() => setCurrentPage(i)}
                    className={currentPage === i ? "active" : ""}
                >
                    {i}
                </button>
            );
        }

        if (currentPage < totalPages - 2) {
            pages.push(<span key="end-ellipsis">...</span>);
        }

        pages.push(
            <button
                key={totalPages}
                onClick={() => setCurrentPage(totalPages)}
                className={currentPage === totalPages ? "active" : ""}
            >
                {totalPages}
            </button>
        );

        return pages;
    };

    return (
        <div className="App">
            <h2>Select an Index</h2>
            <select
                value={selectedIndex || ""}
                onChange={(e) => handleIndexSelect(e.target.value)}
            >
                <option value="" disabled>
                    Select an index
                </option>
                {indices.map((index) => (
                    <option key={index.id} value={index.id}>
                        {index.name}
                    </option>
                ))}
            </select>

            {stockData && selectedCompany && (
                <div className="stock-details">
                    <h3>Stock Details for {selectedCompany}</h3>
                    <p>
                        <strong>Volume:</strong> {stockData.optionDetail.volume}
                    </p>
                    <p>
                        <strong>Average Volume:</strong>{" "}
                        {stockData.optionDetail.avgVolume}
                    </p>
                    <p>
                        <strong>Previous Close:</strong>{" "}
                        {stockData.optionDetail.previousClose}
                    </p>
                    <p>
                        <strong>Open:</strong> {stockData.optionDetail.open}
                    </p>
                    <p>
                        <strong>Day Range:</strong>{" "}
                        {stockData.optionDetail.dayRange.low} -{" "}
                        {stockData.optionDetail.dayRange.high}
                    </p>
                    <p>
                        <strong>52-Week Range:</strong>{" "}
                        {stockData.optionDetail.rangeWeek52.low} -{" "}
                        {stockData.optionDetail.rangeWeek52.high}
                    </p>
                </div>
            )}

            {selectedIndex && (
                <>
                    <h2>Company List for {selectedIndex}</h2>
                    <table className="company-table">
                        <thead>
                            <tr>
                                <th onClick={() => handleSort("rank")}>Rank</th>
                                <th onClick={() => handleSort("companyName")}>
                                    Company Name
                                </th>
                                <th onClick={() => handleSort("symbol")}>
                                    Symbol
                                </th>
                                <th onClick={() => handleSort("weight")}>
                                    Weight
                                </th>
                                <th onClick={() => handleSort("price")}>
                                    Price
                                </th>
                                <th onClick={() => handleSort("change")}>
                                    Change
                                </th>
                                <th
                                    onClick={() =>
                                        handleSort("percentageChange")
                                    }
                                >
                                    % Change
                                </th>
                                <th>Details</th>
                            </tr>
                        </thead>
                        <tbody>
                            {currentCompanies.map((company) => (
                                <tr key={company.rank}>
                                    <td>{company.rank}</td>
                                    <td>{company.companyName}</td>
                                    <td>{company.symbol}</td>
                                    <td>
                                        {(company.weight * 100).toFixed(2)}%
                                    </td>
                                    <td>{company.price}</td>
                                    <td>{company.change}</td>
                                    <td>{company.percentageChange}%</td>
                                    <td>
                                        <button
                                            onClick={() =>
                                                fetchStockData(company.symbol)
                                            }
                                        >
                                            View Stock
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                    <div className="pagination">
                        {renderPaginationButtons()}
                    </div>
                </>
            )}
        </div>
    );
}

export default App;
