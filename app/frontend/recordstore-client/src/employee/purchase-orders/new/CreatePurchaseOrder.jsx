import {
  Box,
  Button,
  Container,
  Flex,
  Heading,
  Input,
  InputGroup,
  InputLeftElement,
  Select,
  TableContainer,
  Table,
  Tbody,
  Td,
  Th,
  Thead,
  Tr,
  Badge,
  Icon,
  Text,
  useToast,
} from '@chakra-ui/react';

import { Link } from 'react-router-dom';

import { FaExternalLinkAlt, FaSearch } from 'react-icons/fa';
import { useState, useRef } from 'react';

import productService from '../../../hooks/ProductService';
import usePurchaseOrders from '../usePurchaseOrders';

import formatCurrency from '../../../utils/formatCurrency';
import formatDate from '../../../utils/formatDate';

const CreatePurchaseOrder = () => {
  const [total, setTotal] = useState(0);
  const [supplierId, setSupplierId] = useState('');
  const [purchaseOrderLines, setPurchaseOrderLines] = useState([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState([]);
  const [currentPage, setCurrentPage] = useState(1);
  const params = useRef({});

  const { suppliers, isSupplierLoading, createPurchaseOrder } =
    usePurchaseOrders(params.current, params.current);

  const toast = useToast();

  const handleSearch = async (e) => {
    const query = e.target.value;
    setSearchQuery(query);

    if (query.trim() === '') {
      setSearchResults([]);
      return;
    }

    const filterParams = {
      title: query,
      page: currentPage,
    };

    try {
      const data = await getProducts(filterParams);
      setSearchResults(data.results);
    } catch (error) {
      console.error('Error fetching products:', error);
    }
  };

  const getProducts = async (filterParams) => {
    const response = await productService.getProducts(filterParams);
    return response;
  };

  const handleSetTotal = (value) => {
    const total = parseInt(value, 10);
    setTotal(total);
  };

  const handleAddLine = (product) => {
    setPurchaseOrderLines([
      ...purchaseOrderLines,
      { productId: product.id, quantity: 1 },
    ]);
    setSearchQuery('');
    setSearchResults([]);
  };

  const isValid = () => {
    return total > 0 && supplierId !== '' && purchaseOrderLines.length > 0;
  };

  const handleQuantityChange = (index, value) => {
    const updatedLines = [...purchaseOrderLines];
    updatedLines[index].quantity = parseInt(value, 10);
    setPurchaseOrderLines(updatedLines);
  };

  const handleRemoveLine = (index) => {
    const updatedLines = [...purchaseOrderLines];
    updatedLines.splice(index, 1);
    setPurchaseOrderLines(updatedLines);
  };

  const handleCreateOrder = async () => {
    const newOrder = {
      total,
      supplierId,
      purchaseOrderLines,
    };

    const response = await createPurchaseOrder(newOrder);
    if (response.success) {
      toast({
        title: 'Закупівля створена',
        description: 'Закупівля успішно створена',
        status: 'success',
        duration: 5000,
        isClosable: true,
      });
    } else {
      toast({
        title: 'Помилка',
        description:
          'Під час створення закупівлі виникла помилка. ' + response.message,
        status: 'error',
        duration: 5000,
        isClosable: true,
      });
    }
  };

  return (
    <Box bg='gray.100' py={12} flexGrow={1}>
      <Container maxW='7xl'>
        <Heading color='teal.600' mb={4}>
          Створити нову закупівлю
        </Heading>
        <Flex mb={4}>
          <InputGroup mr={4}>
            <InputLeftElement pointerEvents='none'>
              <FaSearch color='gray.300' />
            </InputLeftElement>
            <Input
              type='text'
              placeholder='Пошук товарів'
              value={searchQuery}
              onChange={handleSearch}
            />
          </InputGroup>
          {isSupplierLoading ? (
            <Select placeholder='Завантаження постачальників...' isLoading />
          ) : (
            <Select
              mr={4}
              placeholder='Вибрати постачальника'
              value={supplierId}
              onChange={(e) => setSupplierId(e.target.value)}
            >
              {suppliers.map((supplier) => (
                <option key={supplier.id} value={supplier.id}>
                  {supplier.name}
                </option>
              ))}
            </Select>
          )}
          <Input
            type='number'
            placeholder='Загальна сума'
            value={total}
            onChange={(e) => handleSetTotal(e.target.value)}
          />
        </Flex>
        <Box overflowY='auto' maxH='300px' mb={4}>
          {searchResults.map(
            (product) => (
              console.log(product),
              (
                <Flex
                  key={product.id}
                  align='center'
                  mb={4}
                  p={4}
                  bg='white'
                  borderRadius='md'
                  boxShadow='md'
                >
                  <Box flex='1' mr={4}>
                    <Heading size='md' color='teal.600' mb={2}>
                      <Box
                        _hover={{ transform: 'scale(1.02)' }}
                        display={'inline-flex'}
                        transition={'transform 0.3s ease'}
                      >
                        <Link to={`/products/${product.id}`} target='_blank'>
                          {product.title}{' '}
                          <Icon as={FaExternalLinkAlt} boxSize={4} />
                        </Link>
                      </Box>
                    </Heading>
                    <Box>
                      <Badge colorScheme='teal' mr={2}>
                        Ціна: {formatCurrency(product.price)}
                      </Badge>
                      <Badge colorScheme='gray' mr={2}>
                        Реліз: {formatDate(product.releaseDate)}
                      </Badge>
                      <Badge colorScheme='purple' mr={2}>
                        Формат: {product.format.name}
                      </Badge>
                      <Box mt={2}>
                        <Text fontWeight='bold' mb={1}>
                          Жанри:
                        </Text>
                        {product.genres.map((genre) => (
                          <Badge key={genre.name} colorScheme='green' mr={2}>
                            {genre.name}
                          </Badge>
                        ))}
                      </Box>
                      <Box mt={2}>
                        <Text fontWeight='bold' mb={1}>
                          Виконавці:
                        </Text>
                        {product.artists.map((artist) => (
                          <Badge key={artist.id} colorScheme='blue' mr={2}>
                            {artist.name}
                          </Badge>
                        ))}
                      </Box>
                    </Box>
                  </Box>
                  <Button
                    colorScheme='teal'
                    onClick={() => handleAddLine(product)}
                    _hover={{ bg: 'teal.700' }}
                    transition='background-color 0.3s ease'
                  >
                    Додати
                  </Button>
                </Flex>
              )
            )
          )}
        </Box>
        <TableContainer>
          <Table variant='simple'>
            <Thead>
              <Tr>
                <Th>Товар</Th>
                <Th>Кількість</Th>
                <Th></Th>
              </Tr>
            </Thead>
            <Tbody>
              {purchaseOrderLines.map((line, index) => (
                <Tr key={index}>
                  <Td>{line.productId}</Td>
                  <Td>
                    <Input
                      type='number'
                      value={line.quantity}
                      onChange={(e) =>
                        handleQuantityChange(index, e.target.value)
                      }
                      min={1}
                    />
                  </Td>
                  <Td>
                    <Button
                      colorScheme='red'
                      size='sm'
                      onClick={() => handleRemoveLine(index)}
                      _hover={{ bg: 'red.600' }}
                      transition='background-color 0.3s ease'
                    >
                      Видалити
                    </Button>
                  </Td>
                </Tr>
              ))}
            </Tbody>
          </Table>
        </TableContainer>
        <Button
          colorScheme='teal'
          mt={4}
          onClick={handleCreateOrder}
          _hover={{ bg: 'teal.700' }}
          transition='background-color 0.3s ease'
          isDisabled={!isValid()}
        >
          Створити закупівлю
        </Button>
      </Container>
    </Box>
  );
};

export default CreatePurchaseOrder;
